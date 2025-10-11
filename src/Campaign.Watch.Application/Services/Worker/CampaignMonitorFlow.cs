using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Dtos.Client;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Application.Interfaces.Client;
using Campaign.Watch.Application.Interfaces.Read.Campaign;
using Campaign.Watch.Application.Interfaces.Worker;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Entities.Read.Campaign;
using Campaign.Watch.Domain.Enums;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Worker
{
    /// <summary>
    /// Orquestra o fluxo de monitoramento de campanhas, coordenando a busca,
    /// o processamento e a persistência dos dados.
    /// </summary>
    public class CampaignMonitorFlow : ICampaignMonitorFlow
    {
        private readonly IClientApplication _clientApplication;
        private readonly ICampaignMonitorApplication _campaignMonitorApplication;
        private readonly ICampaignApplication _campaignApplication;
        private readonly ICampaignDataProcessor _dataProcessor;
        private readonly ICampaignHealthCalculator _healthCalculator;
        private readonly ILogger<CampaignMonitorFlow> _logger;
        private readonly IMapper _mapper;


        private static DateTime _lastFullSync = DateTime.MinValue;
        private readonly TimeSpan _fullSyncInterval = TimeSpan.FromHours(1);

        public CampaignMonitorFlow(
            IClientApplication clientApplication,
            ICampaignMonitorApplication campaignMonitorApplication,
            ICampaignApplication campaignApplication,
            ICampaignDataProcessor dataProcessor,
            ICampaignHealthCalculator healthCalculator,
            IMapper mapper,
            ILogger<CampaignMonitorFlow> logger)
        {
            _clientApplication = clientApplication;
            _campaignMonitorApplication = campaignMonitorApplication;
            _campaignApplication = campaignApplication;
            _dataProcessor = dataProcessor;
            _healthCalculator = healthCalculator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Ponto de entrada principal para o processo de monitoramento de campanhas.
        /// </summary>       
        public async Task MonitorarCampanhasAsync()
        {
            var now = DateTime.UtcNow;

            if (now - _lastFullSync > _fullSyncInterval)
            {
                _logger.LogInformation("Iniciando ciclo de sincronização completa de novas campanhas.");
                await SincronizarNovasCampanhasAsync();
                _lastFullSync = now;
                _logger.LogInformation("Ciclo de sincronização completa finalizado.");
            }

            _logger.LogInformation("Iniciando ciclo de monitoramento de campanhas pendentes.");
            await MonitorarCampanhasPendentesAsync();
            _logger.LogInformation("Ciclo de monitoramento de campanhas pendentes finalizado.");
        }

        /// <summary>
        /// Processo secundário e mais pesado: busca todas as campanhas de todos os clientes na origem
        /// e insere na base de monitoramento aquelas que ainda não existem.
        /// </summary>
        private async Task SincronizarNovasCampanhasAsync()
        {
            var clientes = (await _clientApplication.GetAllClientsAsync() ?? Enumerable.Empty<ClientResponse>())
                           .Where(c => c.IsActive).ToList();

            foreach (var cliente in clientes)
            {
                var campanhasOrigem = await _campaignMonitorApplication.GetSourceCampaignsByClientAsync(cliente.CampaignConfig.Database);
                if (!campanhasOrigem.Any()) continue;

                foreach (var campanhaOrigem in campanhasOrigem)
                {
                    var campanhaExistente = await _campaignApplication.ObterCampanhaPorIdCampanhaAsync(cliente.Name, campanhaOrigem.Id);
                    if (campanhaExistente == null)
                    {
                        _logger.LogInformation("Nova campanha encontrada na origem: '{CampaignName}'. Sincronizando.", campanhaOrigem.Name);
                        await ProcessarCampanhaUnicaAsync(cliente, campanhaOrigem);
                    }
                }
            }
        }

        /// <summary>
        /// Processo principal e rápido: busca e atualiza apenas as campanhas que já existem
        /// e estão agendadas para monitoramento.
        /// </summary>
        private async Task MonitorarCampanhasPendentesAsync()
        {
            var campanhasParaProcessar = await _campaignApplication.ObterCampanhasParaMonitorarAsync();
            if (!campanhasParaProcessar.Any())
            {
                _logger.LogInformation("Nenhuma campanha pendente de monitoramento encontrada.");
                return;
            }

            _logger.LogInformation("Encontradas {TotalCampanhas} campanhas para processamento.", campanhasParaProcessar.Count());

            foreach (var campanha in campanhasParaProcessar)
            {
                var cliente = await _clientApplication.GetClientByNameAsync(campanha.ClientName);
                if (cliente == null || !cliente.IsActive) continue;

                //var campanhaOrigem = await BuscarCampanhaDaOrigemAsync(cliente, campanha.IdCampaign);
                var campanhaOrigem = await _campaignMonitorApplication.GetSourceCampaignByIdAsync(cliente.CampaignConfig.Database, campanha.IdCampaign);
                if (campanhaOrigem == null)
                {
                    _logger.LogWarning("Campanha '{CampaignName}' (Id: {CampaignId}) não encontrada na origem.", campanha.Name, campanha.IdCampaign);
                    continue;
                }

                try
                {
                    await ProcessarCampanhaUnicaAsync(cliente, campanhaOrigem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao processar a campanha específica '{CampaignName}'.", campanha.Name);
                }
            }
        }

        

        /// <summary>
        /// Orquestra o processamento de uma única campanha, delegando tarefas para outros serviços.
        /// </summary>
        private async Task ProcessarCampanhaUnicaAsync(ClientResponse client, CampaignRead campaignSource)
        {
            _logger.LogDebug("Iniciando processamento da campanha.");

            // 1. Delega o processamento e enriquecimento dos dados
            var campaignToMonitor = await _dataProcessor.ProcessAndEnrichCampaignDataAsync(client, campaignSource);

            // 2. Delega o cálculo de saúde e status
            var healthResult = _healthCalculator.Calculate(campaignToMonitor, DateTime.UtcNow);

            // 3. Atribui os resultados calculados de volta à entidade
            campaignToMonitor.HealthStatus = healthResult.HealthStatus;
            campaignToMonitor.MonitoringStatus = healthResult.MonitoringStatus;
            campaignToMonitor.NextExecutionMonitoring = healthResult.NextExecutionTime;

            _logger.LogDebug("Tipo da campanha identificado como: {TipoCampanha}", healthResult.CampaignType);

            // 4. Orquestra a persistência dos dados
            await SalvarOuAtualizarCampanhaAsync(campaignToMonitor, healthResult.CampaignType);
        }

        /// <summary>
        /// Verifica se uma campanha já existe no banco de dados local e decide se deve criá-la ou atualizá-la.
        /// </summary>
        private async Task SalvarOuAtualizarCampanhaAsync(CampaignEntity campaign, CampaignType tipoCampanha)
        {
            // Busca o registro existente para obter o ObjectId, se houver.
            var campanhaExistente = await _campaignApplication.ObterCampanhaPorIdCampanhaAsync(campaign.ClientName, campaign.IdCampaign);
            var agora = DateTime.UtcNow;

            campaign.LastCheckMonitoring = agora;

            // Se a campanha já existe, preservamos seu ObjectId original.
            // Se não, o ObjectId será gerado pelo MongoDB na inserção (upsert).
            var objectIdParaAtualizar = campanhaExistente?.Id ?? ObjectId.GenerateNewId().ToString();

            // Mapeia a entidade atualizada para o DTO que o serviço de aplicação espera.
            var campaignDto = _mapper.Map<CampaignDetailResponse>(campaign);

            _logger.LogInformation("Persistindo campanha (Upsert) com IdCampaign: {IdCampaign}", campaign.IdCampaign);

            // Chamamos diretamente o Update, que agora fará o Upsert.
            await _campaignApplication.AtualizarCampanhaAsync(objectIdParaAtualizar, campaignDto);

            _logger.LogInformation(
                "Campanha persistida com sucesso. Status de monitoramento: {MonitoringStatus}",
                campaign.MonitoringStatus);
        }
    }
}