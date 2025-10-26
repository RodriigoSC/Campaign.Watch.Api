using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Campaign
{
    public class CampaignApplication : ICampaignApplication
    {
        private readonly ICampaignService _campaignService;
        private readonly IExecutionRepository _executionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignApplication> _logger;

        public CampaignApplication(ICampaignService campaignService, IExecutionRepository executionRepository, IMapper mapper, ILogger<CampaignApplication> logger)
        {
            _campaignService = campaignService;
            _executionRepository = executionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region Campanhas Monitoradas

        public async Task<CampaignMonitoringResponse> ObterCampanhaMonitoradaPorIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var campaign = await _campaignService.ObterCampanhaPorIdAsync(objectId);
            if (campaign == null)
                return null;

            var response = _mapper.Map<CampaignMonitoringResponse>(campaign);

            // Buscar execuções da collection separada
            var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(objectId);
            response.Metrics = await CalcularMetricasCampanhaAsync(campaign, executions);

            return response;
        }

        public async Task<IEnumerable<CampaignMonitoringResponse>> ObterCampanhasMonitoradasAsync(string clientName = null, string monitoringStatus = null,
            bool? hasErrors = null, DateTime? dataInicio = null, DateTime? dataFim = null, int pagina = 1, int tamanhoPagina = 50)
        {
            IEnumerable<CampaignEntity> campaigns;

            // Aplicar filtros (mantém a lógica existente)
            if (!string.IsNullOrEmpty(clientName) && dataInicio.HasValue && dataFim.HasValue)
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteOuDataAsync(
                    clientName, dataInicio.Value, dataFim.Value);
            }
            else if (!string.IsNullOrEmpty(clientName))
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteAsync(clientName);
            }
            else if (dataInicio.HasValue && dataFim.HasValue)
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasPorDataAsync(
                    dataInicio.Value, dataFim.Value);
            }
            else
            {
                campaigns = await _campaignService.ObterCampanhasPaginadasAsync(pagina, tamanhoPagina);
            }

            // Filtrar por status de monitoramento
            if (!string.IsNullOrEmpty(monitoringStatus) &&
                Enum.TryParse<MonitoringStatus>(monitoringStatus, true, out var status))
            {
                campaigns = campaigns.Where(c => c.MonitoringStatus == status);
            }

            // Filtrar por erros
            if (hasErrors.HasValue)
            {
                campaigns = campaigns.Where(c =>
                    c.HealthStatus?.HasIntegrationErrors == hasErrors.Value);
            }

            return await MapearCampanhasComMetricasAsync(campaigns);
        }

        #endregion

        #region Métricas

        public async Task<CampaignMetricsDto> ObterMetricasCampanhaAsync(string campaignId)
        {
            if (!ObjectId.TryParse(campaignId, out var objectId))
                return null;

            var campaign = await _campaignService.ObterCampanhaPorIdAsync(objectId);
            if (campaign == null)
                return null;

            var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(objectId);
            return await CalcularMetricasCampanhaAsync(campaign, executions);
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<IEnumerable<CampaignMonitoringResponse>> MapearCampanhasComMetricasAsync(IEnumerable<CampaignEntity> campaigns)
        {
            var responses = new List<CampaignMonitoringResponse>();

            foreach (var campaign in campaigns)
            {
                var response = _mapper.Map<CampaignMonitoringResponse>(campaign);
                var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(campaign.Id);
                response.Metrics = await CalcularMetricasCampanhaAsync(campaign, executions);
                responses.Add(response);
            }

            return responses;
        }

        private async Task<CampaignMetricsDto> CalcularMetricasCampanhaAsync(CampaignEntity campaign, IEnumerable<ExecutionEntity> executions)
        {
            var validExecutions = executions.Where(e => e.Status != "MissingInSource").ToList();

            var metrics = new CampaignMetricsDto
            {
                TotalExecutions = validExecutions.Count,
                CompletedExecutions = validExecutions.Count(e => e.Status == "Completed"),
                FailedExecutions = validExecutions.Count(e =>
                    e.Status == "Error" || e.HasMonitoringErrors),
                InProgressExecutions = validExecutions.Count(e =>
                    e.Status != "Completed" && e.Status != "Error"),
                LastExecutionDate = validExecutions.Any() ?
                    validExecutions.Max(e => e.StartDate) : null,
                NextScheduledExecution = campaign.NextExecutionMonitoring
            };

            if (metrics.TotalExecutions > 0)
            {
                metrics.SuccessRate = (double)metrics.CompletedExecutions /
                    metrics.TotalExecutions * 100;

                var executionsWithDuration = validExecutions
                    .Where(e => e.TotalDurationInSeconds.HasValue).ToList();

                if (executionsWithDuration.Any())
                {
                    var avgSeconds = executionsWithDuration
                        .Average(e => e.TotalDurationInSeconds.Value);
                    metrics.AverageExecutionTime = TimeSpan.FromSeconds(avgSeconds);
                }
            }

            return metrics;
        }

        #endregion

        #region Métodos Não Implementados (Mantidos)

        public Task<CampaignMonitoringResponse> ObterCampanhaMonitoradaPorIdCampanhaAsync(string clientName, string idCampanha)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMonitoringResponse>> ObterCampanhasMonitoradasPorClienteAsync(string clientName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}