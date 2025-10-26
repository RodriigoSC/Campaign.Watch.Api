using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Dashboard
{
    public class UpcomingExecutionDto
    {
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public DateTime ScheduledFor { get; set; }
        public string CampaignType { get; set; }
    }
}
