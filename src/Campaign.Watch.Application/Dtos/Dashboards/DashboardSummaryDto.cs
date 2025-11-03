using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int CampaignsWithIssues { get; set; }
        public int TotalExecutionsToday { get; set; }
        public int SuccessfulExecutionsToday { get; set; }
        public double OverallHealthScore { get; set; }
    }
}
