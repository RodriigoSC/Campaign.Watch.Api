using AutoMapper;
using Campaign.Watch.Application.Dtos.Diagnostic;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Campaign
{
    public class DiagnosticApplication : IDiagnosticApplication
    {
        private readonly ICampaignService _campaignService;
        private readonly IMapper _mapper;
        private readonly ILogger<DiagnosticApplication> _logger;

        public DiagnosticApplication(ICampaignService campaignService, IMapper mapper, ILogger<DiagnosticApplication> logger)
        {
            _campaignService = campaignService;
            _mapper = mapper;
            _logger = logger;
        }

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

        #region Métodos Auxiliares (Movidos)

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

        #endregion
    }
}
