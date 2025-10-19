using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    public class RecentIssueDto
    {
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string IssueType { get; set; }
        public string Severity { get; set; }
        public DateTime DetectedAt { get; set; }
        public string Description { get; set; }
    }
}
