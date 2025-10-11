using AutoMapper;
using Campaign.Watch.Application.Dtos.Client;
using Campaign.Watch.Application.Helpers;
using Campaign.Watch.Application.Interfaces.Read.Campaign;
using Campaign.Watch.Application.Interfaces.Worker;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Entities.Read.Campaign;
using Campaign.Watch.Domain.Enums;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effmail;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effpush;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effsms;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effwhatsapp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Worker
{
    public class CampaignDataProcessor : ICampaignDataProcessor
    {
        private readonly ICampaignMonitorApplication _campaignMonitorApplication;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignDataProcessor> _logger;
        private readonly IEffmailReadService _effmailReadService;
        private readonly IEffsmsReadService _effsmsReadService;
        private readonly IEffpushReadService _effpushReadService;
        private readonly IEffwhatsappReadService _effwhatsappReadService;

        public CampaignDataProcessor(
            ICampaignMonitorApplication campaignMonitorApplication,
            IMapper mapper,
            ILogger<CampaignDataProcessor> logger,
            IEffmailReadService effmailReadService,
            IEffsmsReadService effsmsReadService,
            IEffpushReadService effpushReadService,
            IEffwhatsappReadService effwhatsappReadService)
        {
            _campaignMonitorApplication = campaignMonitorApplication;
            _mapper = mapper;
            _logger = logger;
            _effmailReadService = effmailReadService;
            _effsmsReadService = effsmsReadService;
            _effpushReadService = effpushReadService;
            _effwhatsappReadService = effwhatsappReadService;
        }

        public async Task<CampaignEntity> ProcessAndEnrichCampaignDataAsync(ClientResponse client, CampaignRead campaignSource)
        {
            var campaignEntity = _mapper.Map<CampaignEntity>(campaignSource);
            campaignEntity.ClientName = client.Name;

            try
            {
                var execucoesOrigem = await _campaignMonitorApplication.GetSourceExecutionsByCampaignAsync(client.CampaignConfig.Database, campaignEntity.IdCampaign)
                                       ?? Enumerable.Empty<ExecutionRead>();

                campaignEntity.Executions = _mapper.Map<List<Execution>>(execucoesOrigem);
                //campaignEntity.Executions = VerificarEAdicionarExecucoesFaltantes(campaignEntity);
                campaignEntity.Executions = VerificarExecucoesFaltantesECalcularLatencia(campaignEntity);

                foreach (var execution in campaignEntity.Executions)
                {
                    if (execution.Status == "MissingInSource") continue;

                    if (execution.EndDate.HasValue)
                    {
                        execution.TotalDurationInSeconds = (execution.EndDate.Value - execution.StartDate).TotalSeconds;
                    }

                    execution.IsFullyVerifiedByMonitoring = true;
                    foreach (var step in execution.Steps)
                    {
                        await EnriquecerStepComDadosDoCanalAsync(client, execution, step);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enriquecer a campanha com dados de execuções. A campanha será processada com dados parciais.");
                if (campaignEntity.Executions == null)
                    campaignEntity.Executions = new List<Execution>();
            }

            return campaignEntity;
        }

        /// <summary>
        /// Compara as execuções reais de uma campanha recorrente com as execuções esperadas
        /// e cria placeholders de erro para qualquer execução ausente.
        /// </summary>
        /// <param name="campaign">A entidade da campanha a ser verificada.</param>
        /// <returns>Uma nova lista de execuções contendo as reais e as "fantasmas" (ausentes).</returns>
        /*private List<Execution> VerificarEAdicionarExecucoesFaltantes(CampaignEntity campaign)
        {
            // Esta lógica só se aplica a campanhas recorrentes, ativas e que já deveriam ter começado.
            if (campaign.Scheduler?.IsRecurrent != true || !campaign.IsActive || DateTime.UtcNow < campaign.Scheduler.StartDateTime)
            {
                return campaign.Executions ?? new List<Execution>();
            }

            var execucoesReais = campaign.Executions ?? new List<Execution>();
            var execucoesCombinadas = new List<Execution>(execucoesReais);

            // Gera todas as ocorrências desde o início da campanha até a data/hora de hoje.
            var datasEsperadas = SchedulerHelper.GetAllOccurrences(
                campaign.Scheduler.Crontab,
                campaign.Scheduler.StartDateTime,
                DateTime.UtcNow
            );

            if (!datasEsperadas.Any())
            {
                return execucoesReais;
            }

            // Usar um HashSet para busca rápida (O(1)) das datas existentes.
            // Comparamos apenas a parte da data (ignorando horas/minutos) para maior flexibilidade.
            var datasReais = new HashSet<DateTime>(execucoesReais.Select(e => e.StartDate.Date));

            foreach (var dataEsperada in datasEsperadas)
            {
                // Ignora execuções agendadas para o futuro (a partir de hoje)
                if (dataEsperada.Date >= DateTime.UtcNow.Date) continue;

                // Se uma data esperada não existe no conjunto de datas reais, ela está faltando.
                if (!datasReais.Contains(dataEsperada.Date))
                {
                    _logger.LogWarning("Execução faltante detectada para a campanha '{CampaignName}' na data {MissingDate}", campaign.Name, dataEsperada.ToShortDateString());
                    var placeholder = CreateMissingExecutionPlaceholder(dataEsperada, campaign.Name);
                    execucoesCombinadas.Add(placeholder);
                }
            }

            // Retorna a lista combinada, ordenada por data para manter a consistência.
            return execucoesCombinadas.OrderBy(e => e.StartDate).ToList();
        }*/
        private List<Execution> VerificarExecucoesFaltantesECalcularLatencia(CampaignEntity campaign)
        {
            if (campaign.Scheduler?.IsRecurrent != true || !campaign.IsActive || DateTime.UtcNow < campaign.Scheduler.StartDateTime)
            {
                return campaign.Executions ?? new List<Execution>();
            }

            var execucoesReais = campaign.Executions ?? new List<Execution>();

            var datasEsperadas = SchedulerHelper.GetAllOccurrences(
                campaign.Scheduler.Crontab,
                campaign.Scheduler.StartDateTime,
                DateTime.UtcNow
            );

            // NOVO: Usar um dicionário para mapear data esperada para a execução real correspondente
            var execucoesPorData = execucoesReais.ToDictionary(e => e.StartDate.Date);

            foreach (var dataEsperada in datasEsperadas)
            {
                if (execucoesPorData.TryGetValue(dataEsperada.Date, out var execucaoReal))
                {
                    // Se encontrou a execução, calcula a latência
                    execucaoReal.StartLatencyInSeconds = (execucaoReal.StartDate - dataEsperada).TotalSeconds;
                }
                else if (dataEsperada.Date < DateTime.UtcNow.Date)
                {
                    _logger.LogWarning("Execução faltante detectada para a campanha '{CampaignName}' na data {MissingDate}", campaign.Name, dataEsperada.ToShortDateString());
                    var placeholder = CreateMissingExecutionPlaceholder(dataEsperada, campaign.Name);
                    execucoesReais.Add(placeholder);
                }
            }

            return execucoesReais.OrderBy(e => e.StartDate).ToList();
        }
        /// <summary>
        /// Cria um objeto de Execução "fantasma" para representar uma execução que estava agendada mas não foi encontrada.
        /// </summary>
        /// <param name="expectedDate">A data em que a execução deveria ter ocorrido.</param>
        /// <param name="campaignName">O nome da campanha para referência.</param>
        /// <returns>Um objeto de Execução configurado como um erro de monitoramento.</returns>
        private Execution CreateMissingExecutionPlaceholder(DateTime expectedDate, string campaignName)
        {
            return new Execution
            {
                // Usamos um ID determinístico para evitar duplicatas se o processo rodar mais de uma vez
                ExecutionId = $"MISSING_EXECUTION_{expectedDate:yyyy-MM-dd}",
                CampaignName = campaignName,
                Status = "MissingInSource", // Um status claro que indica a falha
                StartDate = expectedDate,
                EndDate = null,
                IsFullyVerifiedByMonitoring = false, // Não foi verificado, pois é um erro
                HasMonitoringErrors = true,          // A principal flag que nosso sistema de saúde usa
                Steps = new List<Workflows>
                {
                    new Workflows
                    {
                        Id = "monitoring_check_step",
                        Name = "Verificação de Monitoramento",
                        Status = "Error",
                        MonitoringNotes = $"Execução agendada para {expectedDate:dd/MM/yyyy} não foi encontrada na base de dados de origem."
                    }
                }
            };
        }
        /// <summary>
        /// Busca dados de integração para um "step" (passo) do tipo "Channel".
        /// </summary>
        /// <param name="client">O cliente para buscar as configurações de canal.</param>
        /// <param name="execution">A execução que contém o step.</param>
        /// <param name="step">O step a ser enriquecido.</param>
        private async Task EnriquecerStepComDadosDoCanalAsync(ClientResponse client, Execution execution, Workflows step)
        {
            if (step.Type != "Channel" || string.IsNullOrEmpty(step.ChannelName)) return;

            var channelIdentifier = step.ChannelName;

            if (channelIdentifier == "6")
            {
                channelIdentifier = nameof(ChannelType.EffectiveApi);
            }

            if (channelIdentifier == "5")
            {
                channelIdentifier = nameof(ChannelType.EffectiveWhatsApp);
            }

            if (!Enum.TryParse<ChannelType>(step.ChannelName, true, out var channelType))
            {
                DefinirErroDeMonitoramento(execution, step, $"Tipo de canal '{step.ChannelName}' desconhecido.");
                return;
            }

            var channelConfig = client.EffectiveChannels.FirstOrDefault(c => c.TypeChannel == channelType);
            if (channelConfig == null)
            {
                DefinirErroDeMonitoramento(execution, step, $"Configuração para o canal '{step.ChannelName}' não encontrada no cliente.");
                return;
            }

            try
            {
                IntegrationDataBase integrationData = null;
                switch (channelConfig.TypeChannel)
                {
                    case ChannelType.EffectiveMail:
                        var effmailData = await _effmailReadService.GetTriggerEffmail(channelConfig.Database, step.Id);
                        var effmailTrigger = effmailData.FirstOrDefault();
                        if (effmailTrigger != null)
                        {
                            integrationData = _mapper.Map<EmailIntegrationData>(effmailTrigger);
                        }
                        else
                        {
                            step.MonitoringNotes = "Nenhum dado de trigger encontrado para EffectiveMail.";
                        }
                        break;
                    case ChannelType.EffectiveSms:
                        var effsmsData = await _effsmsReadService.GetTriggerEffsms(channelConfig.Database, step.Id);
                        var effsmsTrigger = effsmsData.FirstOrDefault();
                        if (effsmsTrigger != null)
                        {
                            integrationData = _mapper.Map<SmsIntegrationData>(effsmsTrigger);
                        }
                        else
                        {
                            step.MonitoringNotes = "Nenhum dado de trigger encontrado para EffectiveSms.";
                        }
                        break;
                    case ChannelType.EffectivePush:
                        var effpushData = await _effpushReadService.GetTriggerEffpush(channelConfig.Database, step.Id);
                        var effpushTrigger = effpushData.FirstOrDefault();
                        if (effpushTrigger != null)
                        {
                            integrationData = _mapper.Map<PushIntegrationData>(effpushTrigger);
                        }
                        else
                        {
                            step.MonitoringNotes = "Nenhum dado de trigger encontrado para EffectivePush.";
                        }
                        break;
                    case ChannelType.EffectiveWhatsApp:
                        var effwhatsappData = await _effwhatsappReadService.GetTriggerEffwhatsapp(channelConfig.Database, step.Id);
                        var effwhatsappTrigger = effwhatsappData.FirstOrDefault();
                        if (effwhatsappTrigger != null)
                        {
                            integrationData = _mapper.Map<WhatsAppIntegrationData>(effwhatsappTrigger);
                        }
                        else
                        {
                            step.MonitoringNotes = "Nenhum dado de trigger encontrado para EffectiveWhatsApp.";
                        }

                        break;
                    case ChannelType.EffectiveApi:
                        step.MonitoringNotes = "Busca de dados de API (EffectiveApi) ainda não implementada.";
                        break;
                    default:
                        step.MonitoringNotes = $"Nenhum serviço de leitura de dados implementado para o canal {channelConfig.TypeChannel}.";
                        break;
                }
                step.IntegrationData = integrationData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar dados de integração para o step '{StepName}' do canal {ChannelType}.", step.Name, channelConfig.TypeChannel);
                DefinirErroDeMonitoramento(execution, step, $"Falha ao buscar dados de integração: {ex.Message}");
            }
        }
        /// <summary>
        /// Define as flags e notas de erro em uma execução e seu step.
        /// </summary>
        /// <param name="execution">A execução a ser marcada com erro.</param>
        /// <param name="step">O step onde o erro ocorreu.</param>
        /// <param name="errorMessage">A mensagem de erro.</param>
        private void DefinirErroDeMonitoramento(Execution execution, Workflows step, string errorMessage)
        {
            step.MonitoringNotes = errorMessage;
            execution.HasMonitoringErrors = true;
            execution.IsFullyVerifiedByMonitoring = false; // Se houve um erro, não foi totalmente verificado.
            _logger.LogWarning("Erro de monitoramento no step '{StepName}': {ErrorMessage}", step.Name, errorMessage);
        }
    }
}