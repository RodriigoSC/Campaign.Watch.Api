using Campaign.Watch.Application.Helpers;
using Campaign.Watch.Application.Interfaces.Worker;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using System;
using System.Linq;

namespace Campaign.Watch.Application.Services.Worker
{
    public class CampaignHealthCalculator : ICampaignHealthCalculator
    {
        public CampaignHealthResult Calculate(CampaignEntity campaign, DateTime now)
        {
            var campaignType = DeterminarTipoCampanha(campaign);
            var nextExecution = CalcularProximaExecucao(campaign, campaignType, now);

            campaign.NextExecutionMonitoring = nextExecution;

            var healthStatus = CalcularStatusDeSaude(campaign, campaignType, now);
            var monitoringStatus = DeterminarStatusMonitoramento(campaign, healthStatus, campaignType);

            return new CampaignHealthResult(healthStatus, monitoringStatus, campaignType, nextExecution);
        }

        private CampaignType DeterminarTipoCampanha(CampaignEntity campaign)
        {
            return (campaign.Scheduler?.IsRecurrent == true || (campaign.Executions?.Count ?? 0) > 1)
                ? CampaignType.Recorrente
                : CampaignType.Pontual;
        }

        private DateTime? CalcularProximaExecucao(CampaignEntity campaign, CampaignType tipoCampanha, DateTime now)
        {            
            // Se a campanha tem uma data de término e essa data já passou, não há próxima execução.
            if (campaign.Scheduler?.EndDateTime.HasValue == true && now > campaign.Scheduler.EndDateTime.Value)
            {
                return null;
            }

            if (tipoCampanha == CampaignType.Recorrente && campaign.Scheduler?.IsRecurrent == true && !string.IsNullOrWhiteSpace(campaign.Scheduler.Crontab))
            {
                return SchedulerHelper.GetNextExecution(campaign.Scheduler.Crontab, now);
            }
            return campaign.Scheduler?.StartDateTime;
        }

        private MonitoringHealthStatus CalcularStatusDeSaude(CampaignEntity campaign, CampaignType tipoCampanha, DateTime now)
        {
            var healthStatus = new MonitoringHealthStatus();
            
            healthStatus.IsFullyVerified = (campaign.Executions?.Any() == true) && (campaign.Executions.All(e => e.IsFullyVerifiedByMonitoring));

            var execucoesComErro = campaign.Executions?.Where(e => e.HasMonitoringErrors).ToList();

            healthStatus.HasIntegrationErrors = execucoesComErro?.Any() ?? false;
            if (healthStatus.HasIntegrationErrors)
            {
                var ultimaExecucaoComErro = execucoesComErro.OrderBy(e => e.StartDate).Last();
                healthStatus.LastExecutionWithIssueId = ultimaExecucaoComErro.ExecutionId;
                healthStatus.LastMessage = ultimaExecucaoComErro.Steps?.FirstOrDefault(s => !string.IsNullOrEmpty(s.MonitoringNotes))?.MonitoringNotes ?? "Erro de integração detectado.";
            }

            // Se não houver erros de integração, verificamos se há um "Wait" ativo.            
            if (!healthStatus.HasIntegrationErrors)
            {
                // NOVO: Adiciona a verificação de steps travados ANTES de verificar a etapa de espera
                VerificarStepsTravados(campaign, healthStatus, now);

                // A verificação de "Wait" só ocorre se nenhum step estiver travado
                if (!healthStatus.HasIntegrationErrors)
                {
                    VerificarEtapaDeEsperaAtiva(campaign, healthStatus);
                }
            }

            if (!healthStatus.HasIntegrationErrors)
            {
                healthStatus.LastMessage = "Campanha monitorada sem problemas aparentes.";
            }

            return healthStatus;
        }

        /// <summary>
        /// Verifica se há algum step em execução por um tempo excessivo(1 hora).
        /// </summary>
        private void VerificarStepsTravados(CampaignEntity campaign, MonitoringHealthStatus healthStatus, DateTime now)
        {
            var ultimaExecucao = campaign.Executions?.Where(e => e.Status != "MissingInSource").OrderBy(e => e.StartDate).LastOrDefault();

            // A verificação só se aplica se a campanha inteira estiver com status de "Executing"
            if (ultimaExecucao == null || campaign.StatusCampaign != CampaignStatus.Executing)
            {
                return;
            }

            var limiteDeTempo = TimeSpan.FromHours(1);

            foreach (var step in ultimaExecucao.Steps)
            {
                // A regra se aplica a qualquer step (que não seja "Wait") que esteja com status "Running"
                if (step.Type != "Wait" && step.Status == "Running")
                {
                    // O tempo de execução do step é a diferença entre agora e o início da *execução*
                    var tempoEmExecucao = now - ultimaExecucao.StartDate;

                    if (tempoEmExecucao > limiteDeTempo)
                    {
                        // Problema encontrado!
                        healthStatus.HasIntegrationErrors = true;
                        healthStatus.LastExecutionWithIssueId = ultimaExecucao.ExecutionId;
                        healthStatus.LastMessage = $"Alerta de Performance: O step '{step.Name}' está em execução há mais de {limiteDeTempo.TotalHours} hora(s).";

                        // Marca a execução com um erro de monitoramento para que a flag geral seja acionada
                        ultimaExecucao.HasMonitoringErrors = true;
                        step.MonitoringNotes = healthStatus.LastMessage;

                        return;
                    }
                }
            }
        }

        private MonitoringStatus DeterminarStatusMonitoramento(CampaignEntity campaign, MonitoringHealthStatus healthStatus, CampaignType campaignType)
        {
            if (healthStatus.HasIntegrationErrors) return MonitoringStatus.Failed;
            if (healthStatus.HasPendingExecution) return MonitoringStatus.ExecutionDelayed;

            // Verifica se a campanha recorrente já terminou
            if (campaignType == CampaignType.Recorrente &&
                campaign.Scheduler?.EndDateTime.HasValue == true &&
                DateTime.UtcNow > campaign.Scheduler.EndDateTime.Value)
            {
                // Se a última execução foi concluída, então a campanha inteira está concluída.
                if (campaign.StatusCampaign == CampaignStatus.Completed)
                {
                    return MonitoringStatus.Completed;
                }
            }

            switch (campaign.StatusCampaign)
            {
                case CampaignStatus.Completed:
                    if (campaignType == CampaignType.Recorrente)
                        return MonitoringStatus.WaitingForNextExecution;
                    return healthStatus.IsFullyVerified ? MonitoringStatus.Completed : MonitoringStatus.InProgress;

                case CampaignStatus.Error:
                case CampaignStatus.Canceled:
                    return MonitoringStatus.Failed;
                case CampaignStatus.Executing:
                    return MonitoringStatus.InProgress;
                case CampaignStatus.Scheduled:
                    return MonitoringStatus.Pending;
                default:
                    return MonitoringStatus.Pending;
            }
        }

        /// <summary>
        /// Verifica se a campanha está em um estado de espera legítimo devido a um componente "Wait" ativo.
        /// </summary>
        private void VerificarEtapaDeEsperaAtiva(CampaignEntity campaign, MonitoringHealthStatus healthStatus)
        {
            // Só executa essa verificação se a campanha estiver em execução e sem outros erros já detectados.
            if (campaign.StatusCampaign != CampaignStatus.Executing || healthStatus.HasIntegrationErrors)
            {
                return;
            }

            var lastExecution = campaign.Executions?.OrderBy(e => e.StartDate).LastOrDefault();
            if (lastExecution?.Steps == null) return;

            // Procura por um passo de "Wait" que esteja atualmente em execução ("Running")
            var activeWaitStep = lastExecution.Steps.FirstOrDefault(s => s.Type == "Wait" && s.Status == "Running");

            if (activeWaitStep != null && activeWaitStep.TotalExecutionTime > 0)
            {
                // ASSUMINDO que TotalExecutionTime está em segundos. Se for milissegundos, use TimeSpan.FromMilliseconds.
                // Acesso direto à propriedade, sem o .Value
                var waitDuration = TimeSpan.FromSeconds(activeWaitStep.TotalExecutionTime);

                // A melhor estimativa que temos para o início da espera é o início da própria execução.
                var expectedEndTime = lastExecution.StartDate.Add(waitDuration);

                // Atualiza a mensagem de saúde com uma informação clara e útil.
                healthStatus.LastMessage = $"Em andamento. Aguardando componente de espera ('{activeWaitStep.Name}') até aprox. {expectedEndTime:dd/MM/yyyy HH:mm}.";

                // Garante que essa condição "saudável" não seja marcada como um problema de execução pendente.
                healthStatus.HasPendingExecution = false;
            }
        }
    }
}
