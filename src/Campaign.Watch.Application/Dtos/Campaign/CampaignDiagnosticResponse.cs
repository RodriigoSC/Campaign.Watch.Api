using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// Resposta de diagnóstico completo
    /// </summary>
    public class CampaignDiagnosticResponse
    {
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public DateTime AnalyzedAt { get; set; }
        public string OverallStatus { get; set; }
        public List<DiagnosticIssueDto> Issues { get; set; }
        public List<DiagnosticRecommendationDto> Recommendations { get; set; }
    }
}
