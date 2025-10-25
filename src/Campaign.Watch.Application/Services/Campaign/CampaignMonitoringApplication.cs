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
    public class CampaignMonitoringApplication : ICampaignMonitoringApplication
    {
        private readonly ICampaignService _campaignService;
        private readonly IExecutionRepository _executionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignMonitoringApplication> _logger;

        public CampaignMonitoringApplication(ICampaignService campaignService, IExecutionRepository executionRepository, IMapper mapper, ILogger<CampaignMonitoringApplication> logger)
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

        #region Execuções - ATUALIZADO para usar ExecutionRepository

        public async Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesPorCampanhaAsync(string campaignMonitoringId)
        {
            if (!ObjectId.TryParse(campaignMonitoringId, out var objectId))
                return Enumerable.Empty<ExecutionMonitoringResponse>();

            var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(objectId);

            return executions.Select(exec =>
            {
                var response = _mapper.Map<ExecutionMonitoringResponse>(exec);
                response.HealthSummary = CalcularResumoSaudeExecucao(exec);
                return response;
            });
        }

        public async Task<ExecutionMonitoringResponse> ObterExecucaoPorIdAsync(string executionId)
        {
            var execution = await _executionRepository.ObterExecucaoPorIdAsync(executionId);

            if (execution == null)
                return null;

            var response = _mapper.Map<ExecutionMonitoringResponse>(execution);
            response.HealthSummary = CalcularResumoSaudeExecucao(execution);
            return response;
        }

        public async Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesComErrosAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var executions = await _executionRepository.ObterExecucoesComErrosAsync(
                clientName, dataInicio, dataFim);

            return executions.Select(exec =>
            {
                var response = _mapper.Map<ExecutionMonitoringResponse>(exec);
                response.HealthSummary = CalcularResumoSaudeExecucao(exec);
                return response;
            });
        }

        #endregion

        #region Diagnósticos

        public async Task<CampaignDiagnosticResponse> ObterDiagnosticoCampanhaAsync(string campaignId)
        {
            if (!ObjectId.TryParse(campaignId, out var objectId))
                return null;

            var campaign = await _campaignService.ObterCampanhaPorIdAsync(objectId);
            if (campaign == null)
                return null;

            var diagnostic = new CampaignDiagnosticResponse
            {
                CampaignId = campaign.Id.ToString(),
                CampaignName = campaign.Name,
                AnalyzedAt = DateTime.UtcNow,
                Issues = new List<DiagnosticIssueDto>(),
                Recommendations = new List<DiagnosticRecommendationDto>()
            };

            // Analisar problemas
            AnalisarProblemasCampanha(campaign, diagnostic);

            // Gerar recomendações
            GerarRecomendacoes(campaign, diagnostic);

            // Determinar status geral
            diagnostic.OverallStatus = DeterminarStatusGeral(diagnostic.Issues);

            return diagnostic;
        }

        public async Task<IEnumerable<DiagnosticIssueDto>> ObterProblemasDetectadosAsync(string severity = null, DateTime? desde = null, int limite = 100)
        {
            var campaigns = await _campaignService.ObterTodasAsCampanhasAsync();
            var issues = new List<DiagnosticIssueDto>();

            foreach (var campaign in campaigns.Take(limite))
            {
                if (campaign.HealthStatus?.HasIntegrationErrors == true)
                {
                    var issue = new DiagnosticIssueDto
                    {
                        Type = "IntegrationError",
                        Severity = "Error",
                        Description = campaign.HealthStatus.LastMessage,
                        Location = $"Campaign: {campaign.Name}",
                        DetectedAt = campaign.LastCheckMonitoring ?? DateTime.UtcNow
                    };

                    if (desde.HasValue && issue.DetectedAt < desde.Value)
                        continue;

                    if (!string.IsNullOrEmpty(severity) && issue.Severity != severity)
                        continue;

                    issues.Add(issue);
                }

                if (campaign.HealthStatus?.HasPendingExecution == true)
                {
                    var issue = new DiagnosticIssueDto
                    {
                        Type = "ExecutionDelayed",
                        Severity = "Warning",
                        Description = "Execução pendente ou atrasada",
                        Location = $"Campaign: {campaign.Name}",
                        DetectedAt = campaign.LastCheckMonitoring ?? DateTime.UtcNow
                    };

                    if (desde.HasValue && issue.DetectedAt < desde.Value)
                        continue;

                    if (!string.IsNullOrEmpty(severity) && issue.Severity != severity)
                        continue;

                    issues.Add(issue);
                }
            }

            return issues.OrderByDescending(i => i.DetectedAt).Take(limite);
        }

        #endregion

        #region Métricas e Dashboard

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

        public async Task<MonitoringDashboardResponse> ObterDadosDashboardAsync(string clientName = null)
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

            var campaignsList = campaigns.ToList();
            var today = DateTime.UtcNow.Date;

            // Buscar todas as execuções de hoje
            var allExecutionsToday = new List<ExecutionEntity>();
            foreach (var campaign in campaignsList)
            {
                var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(campaign.Id);
                allExecutionsToday.AddRange(executions.Where(e => e.StartDate?.Date == today));
            }

            var dashboard = new MonitoringDashboardResponse
            {
                GeneratedAt = DateTime.UtcNow,
                Summary = new DashboardSummaryDto
                {
                    TotalCampaigns = campaignsList.Count,
                    ActiveCampaigns = campaignsList.Count(c => c.IsActive),
                    CampaignsWithIssues = campaignsList.Count(c =>
                        c.HealthStatus?.HasIntegrationErrors == true),
                    TotalExecutionsToday = allExecutionsToday.Count,
                    SuccessfulExecutionsToday = allExecutionsToday.Count(e => !e.HasMonitoringErrors),
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

        #endregion

        #region Estatísticas

        public async Task<IEnumerable<CampaignStatusGroupDto>> ObterContagemPorStatusMonitoramentoAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var counts = await _campaignService.ObterCampanhasPorStatusMonitoramentoAsync(
                clientName, dataInicio, dataFim);

            var total = counts.Sum(c => c.Count);

            return counts.Select(c => new CampaignStatusGroupDto
            {
                Status = c.Status.ToString(),
                Count = c.Count,
                Percentage = total > 0 ? (double)c.Count / total * 100 : 0
            });
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

        private ExecutionHealthSummaryDto CalcularResumoSaudeExecucao(ExecutionEntity execution)
        {
            var steps = execution.Steps ?? Enumerable.Empty<WorkflowStepEntity>();

            return new ExecutionHealthSummaryDto
            {
                OverallHealth = execution.HasMonitoringErrors ? "Error" : "Healthy",
                TotalSteps = steps.Count(),
                HealthySteps = steps.Count(s =>
                    string.IsNullOrEmpty(s.Error) && string.IsNullOrEmpty(s.MonitoringNotes)),
                StepsWithWarnings = steps.Count(s =>
                    !string.IsNullOrEmpty(s.MonitoringNotes) && string.IsNullOrEmpty(s.Error)),
                StepsWithErrors = steps.Count(s => !string.IsNullOrEmpty(s.Error)),
                CriticalSteps = 0,
                MainIssues = steps
                    .Where(s => !string.IsNullOrEmpty(s.MonitoringNotes) ||
                               !string.IsNullOrEmpty(s.Error))
                    .Select(s => s.MonitoringNotes ?? s.Error)
                    .Take(5)
                    .ToList()
            };
        }

        private void AnalisarProblemasCampanha(CampaignEntity campaign, CampaignDiagnosticResponse diagnostic)
        {
            if (campaign.HealthStatus?.HasIntegrationErrors == true)
            {
                diagnostic.Issues.Add(new DiagnosticIssueDto
                {
                    Type = "IntegrationError",
                    Severity = "Error",
                    Description = campaign.HealthStatus.LastMessage,
                    Location = "Campaign Health Status",
                    DetectedAt = campaign.LastCheckMonitoring ?? DateTime.UtcNow
                });
            }

            if (campaign.HealthStatus?.HasPendingExecution == true)
            {
                diagnostic.Issues.Add(new DiagnosticIssueDto
                {
                    Type = "PendingExecution",
                    Severity = "Warning",
                    Description = "Há execução pendente ou atrasada",
                    Location = "Campaign Scheduler",
                    DetectedAt = DateTime.UtcNow
                });
            }

            if (!campaign.IsActive && campaign.Scheduler != null &&
                DateTime.UtcNow >= campaign.Scheduler.StartDateTime)
            {
                diagnostic.Issues.Add(new DiagnosticIssueDto
                {
                    Type = "InactiveCampaign",
                    Severity = "Warning",
                    Description = "Campanha inativa mas dentro do período de execução",
                    Location = "Campaign Status",
                    DetectedAt = DateTime.UtcNow
                });
            }
        }

        private void GerarRecomendacoes(CampaignEntity campaign, CampaignDiagnosticResponse diagnostic)
        {
            if (campaign.HealthStatus?.HasIntegrationErrors == true)
            {
                diagnostic.Recommendations.Add(new DiagnosticRecommendationDto
                {
                    Title = "Verificar Integração",
                    Description = "Há problemas de integração detectados. " +
                                 "Verifique as configurações de canal e conectividade.",
                    Priority = "High"
                });
            }

            if (campaign.ExecutionsWithErrors > 0 && campaign.TotalExecutionsProcessed > 10)
            {
                var errorRate = (double)campaign.ExecutionsWithErrors /
                               campaign.TotalExecutionsProcessed;

                if (errorRate > 0.5)
                {
                    diagnostic.Recommendations.Add(new DiagnosticRecommendationDto
                    {
                        Title = "Alta Taxa de Falha",
                        Description = $"Mais de 50% das execuções falharam " +
                                     $"({campaign.ExecutionsWithErrors} de " +
                                     $"{campaign.TotalExecutionsProcessed}). " +
                                     "Revise a configuração da campanha.",
                        Priority = "High"
                    });
                }
            }
        }

        private string DeterminarStatusGeral(List<DiagnosticIssueDto> issues)
        {
            if (!issues.Any())
                return "Healthy";

            if (issues.Any(i => i.Severity == "Error"))
                return "Error";

            if (issues.Any(i => i.Severity == "Warning"))
                return "Warning";

            return "Healthy";
        }

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