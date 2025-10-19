using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// Resposta de dashboard com métricas agregadas
    /// </summary>
    public class MonitoringDashboardResponse
    {
        public DateTime GeneratedAt { get; set; }
        public DashboardSummaryDto Summary { get; set; }
        public List<CampaignStatusGroupDto> CampaignsByStatus { get; set; }
        public List<CampaignHealthGroupDto> CampaignsByHealth { get; set; }
        public List<RecentIssueDto> RecentIssues { get; set; }
        public List<UpcomingExecutionDto> UpcomingExecutions { get; set; }
    }
}
