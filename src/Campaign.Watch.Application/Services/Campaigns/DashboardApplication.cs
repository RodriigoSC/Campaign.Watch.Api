using AutoMapper;
using Campaign.Watch.Application.Dtos.Dashboard;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Campaign
{
    public class DashboardApplication : IDashboardApplication
    {
        private readonly ICampaignService _campaignService;
        private readonly IExecutionRepository _executionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardApplication> _logger;

        public DashboardApplication(ICampaignService campaignService, IExecutionRepository executionRepository, IMapper mapper, ILogger<DashboardApplication> logger)
        {
            _campaignService = campaignService;
            _executionRepository = executionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MonitoringDashboardResponse> ObterDadosDashboardAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            IEnumerable<CampaignEntity> campaigns;

            bool hasClient = !string.IsNullOrEmpty(clientName);
            bool hasDate = dataInicio.HasValue && dataFim.HasValue;

            if (hasClient && hasDate)
            {
                _logger.LogInformation("Buscando dados do dashboard por cliente {ClientName} e período {DataInicio} a {DataFim}", clientName, dataInicio, dataFim);
                campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteOuDataAsync(clientName, dataInicio.Value, dataFim.Value);
            }
            else if (hasClient)
            {
                _logger.LogInformation("Buscando dados do dashboard por cliente {ClientName}", clientName);
                campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteAsync(clientName);
            }
            else if (hasDate)
            {
                _logger.LogInformation("Buscando dados do dashboard por período {DataInicio} a {DataFim}", dataInicio, dataFim);
                campaigns = await _campaignService.ObterTodasAsCampanhasPorDataAsync(dataInicio.Value, dataFim.Value);
            }
            else
            {
                _logger.LogInformation("Buscando dados do dashboard sem filtros.");
                campaigns = await _campaignService.ObterTodasAsCampanhasAsync();
            }

            var campaignsList = campaigns.ToList();
            
            var startDateFilter = dataInicio?.Date ?? DateTime.UtcNow.Date;
            var endDateFilter = dataFim?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1); 
            
            var campaignIds = campaignsList.Select(c => c.Id).ToList();
            var allExecutionsInPeriod = await _executionRepository.ObterExecucoesPorCampanhasEDataAsync(campaignIds, startDateFilter, endDateFilter);
            var dashboard = new MonitoringDashboardResponse
            {
                GeneratedAt = DateTime.UtcNow,
                Summary = new DashboardSummaryDto
                {
                    TotalCampaigns = campaignsList.Count,
                    ActiveCampaigns = campaignsList.Count(c => c.IsActive),
                    CampaignsWithIssues = campaignsList.Count(c =>
                        c.HealthStatus?.HasIntegrationErrors == true),
                    TotalExecutionsToday = allExecutionsInPeriod.Count(),
                    SuccessfulExecutionsToday = allExecutionsInPeriod.Count(e => !e.HasMonitoringErrors),

                    OverallHealthScore = CalcularScoreSaudeGeral(campaignsList)
                },
                CampaignsByStatus = GerarGruposPorStatus(campaignsList),
                CampaignsByHealth = GerarGruposPorSaude(campaignsList),
                RecentIssues = await GerarProblemasRecentesAsync(campaignsList),
                UpcomingExecutions = await GerarProximasExecucoesAsync(campaignsList)
            };

            return dashboard;
        }

        public async Task<IEnumerable<UpcomingExecutionDto>> ObterProximasExecucoesAsync(int proximasHoras = 24)
        {
            var campaigns = await _campaignService.ObterCampanhasAtivasAsync();
            var now = DateTime.UtcNow;
            var limit = now.AddHours(proximasHoras);

            var upcomingExecutions = new List<UpcomingExecutionDto>();

            foreach (var campaign in campaigns)
            {
                if (campaign.NextExecutionMonitoring.HasValue &&
                    campaign.NextExecutionMonitoring.Value >= now &&
                    campaign.NextExecutionMonitoring.Value <= limit)
                {
                    upcomingExecutions.Add(new UpcomingExecutionDto
                    {
                        CampaignId = campaign.Id.ToString(),
                        CampaignName = campaign.Name,
                        ScheduledFor = campaign.NextExecutionMonitoring.Value,
                        CampaignType = campaign.CampaignType.ToString()
                    });
                }
            }

            return upcomingExecutions.OrderBy(e => e.ScheduledFor);
        }

        public async Task<IEnumerable<CampaignStatusGroupDto>> ObterContagemPorStatusMonitoramentoAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            /*var counts = await _campaignService.ObterCampanhasPorStatusMonitoramentoAsync(
                clientName, dataInicio, dataFim);

            var total = counts.Sum(c => c.Count);

            return counts.Select(c => new CampaignStatusGroupDto
            {
                Status = c.Status.ToString(),
                Count = c.Count,
                Percentage = total > 0 ? (double)c.Count / total * 100 : 0
            });*/
            return null;
        }

        public async Task<IEnumerable<CampaignHealthGroupDto>> ObterContagemPorNivelSaudeAsync(string clientName = null)
        {
            IEnumerable<CampaignEntity> campaigns;

            if (!string.IsNullOrEmpty(clientName))
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteAsync(clientName);
            }
            else
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasAsync();
            }

            var groups = campaigns
                .GroupBy(c => DeterminarNivelSaude(c))
                .Select(g => new CampaignHealthGroupDto
                {
                    HealthLevel = g.Key,
                    Count = g.Count(),
                    CampaignIds = g.Select(c => c.Id.ToString()).ToList()
                })
                .OrderBy(g => g.HealthLevel);

            return groups;
        }

        public async Task<Dictionary<string, double>> ObterTaxaSucessoExecucoesAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            IEnumerable<CampaignEntity> campaigns;

            if (!string.IsNullOrEmpty(clientName))
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteAsync(clientName);
            }
            else
            {
                campaigns = await _campaignService.ObterTodasAsCampanhasAsync();
            }

            var allExecutions = new List<ExecutionEntity>();
            foreach (var campaign in campaigns)
            {
                var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(campaign.Id);
                allExecutions.AddRange(executions.Where(e => e.Status != "MissingInSource"));
            }

            if (dataInicio.HasValue)
            {
                allExecutions = allExecutions.Where(e => e.StartDate >= dataInicio.Value).ToList();
            }

            if (dataFim.HasValue)
            {
                allExecutions = allExecutions.Where(e => e.StartDate <= dataFim.Value).ToList();
            }

            var total = allExecutions.Count;
            var successful = allExecutions.Count(e => !e.HasMonitoringErrors && e.Status == "Completed");
            var withErrors = allExecutions.Count(e => e.HasMonitoringErrors);

            return new Dictionary<string, double>
            {
                ["TotalExecutions"] = total,
                ["SuccessfulExecutions"] = successful,
                ["ExecutionsWithErrors"] = withErrors,
                ["SuccessRate"] = total > 0 ? (double)successful / total * 100 : 0,
                ["ErrorRate"] = total > 0 ? (double)withErrors / total * 100 : 0
            };
        }


        #region Métodos Auxiliares (Movidos para cá)

        private double CalcularScoreSaudeGeral(List<CampaignEntity> campaigns)
        {
            if (!campaigns.Any())
                return 100;

            var totalScore = 0.0;

            foreach (var campaign in campaigns)
            {
                var score = 100.0;

                if (campaign.HealthStatus?.HasIntegrationErrors == true)
                    score -= 50;

                if (campaign.HealthStatus?.HasPendingExecution == true)
                    score -= 20;

                if (!campaign.IsActive)
                    score -= 10;

                totalScore += Math.Max(0, score);
            }

            return totalScore / campaigns.Count;
        }

        private List<CampaignStatusGroupDto> GerarGruposPorStatus(List<CampaignEntity> campaigns)
        {
            var total = campaigns.Count;

            return campaigns
                .GroupBy(c => c.MonitoringStatus)
                .Select(g => new CampaignStatusGroupDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = total > 0 ? (double)g.Count() / total * 100 : 0
                })
                .OrderBy(g => g.Status)
                .ToList();
        }

        private List<CampaignHealthGroupDto> GerarGruposPorSaude(List<CampaignEntity> campaigns)
        {
            return campaigns
                .GroupBy(c => DeterminarNivelSaude(c))
                .Select(g => new CampaignHealthGroupDto
                {
                    HealthLevel = g.Key,
                    Count = g.Count(),
                    CampaignIds = g.Select(c => c.Id.ToString()).ToList()
                })
                .OrderBy(g => g.HealthLevel)
                .ToList();
        }

        private async Task<List<RecentIssueDto>> GerarProblemasRecentesAsync(
            List<CampaignEntity> campaigns)
        {
            var issues = new List<RecentIssueDto>();

            foreach (var campaign in campaigns
                .Where(c => c.HealthStatus?.HasIntegrationErrors == true).Take(10))
            {
                issues.Add(new RecentIssueDto
                {
                    CampaignId = campaign.Id.ToString(),
                    CampaignName = campaign.Name,
                    IssueType = "IntegrationError",
                    Severity = "Error",
                    DetectedAt = campaign.LastCheckMonitoring ?? DateTime.UtcNow,
                    Description = campaign.HealthStatus.LastMessage
                });
            }

            return issues.OrderByDescending(i => i.DetectedAt).ToList();
        }

        private async Task<List<UpcomingExecutionDto>> GerarProximasExecucoesAsync(List<CampaignEntity> campaigns)
        {
            var now = DateTime.UtcNow;
            var limit = now.AddHours(24);

            return campaigns
                .Where(c => c.NextExecutionMonitoring.HasValue &&
                           c.NextExecutionMonitoring.Value >= now &&
                           c.NextExecutionMonitoring.Value <= limit)
                .Select(c => new UpcomingExecutionDto
                {
                    CampaignId = c.Id.ToString(),
                    CampaignName = c.Name,
                    ScheduledFor = c.NextExecutionMonitoring.Value,
                    CampaignType = c.CampaignType.ToString()
                })
                .OrderBy(e => e.ScheduledFor)
                .ToList();
        }

        private string DeterminarNivelSaude(CampaignEntity campaign)
        {
            if (campaign.HealthStatus?.HasIntegrationErrors == true)
                return "Critical";

            if (campaign.HealthStatus?.HasPendingExecution == true)
                return "Warning";

            if (!campaign.IsActive)
                return "Inactive";

            return "Healthy";
        }

        #endregion
    }
}
