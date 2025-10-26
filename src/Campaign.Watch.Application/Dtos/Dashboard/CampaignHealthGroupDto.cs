using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Dashboard
{
    public class CampaignHealthGroupDto
    {
        public string HealthLevel { get; set; }
        public int Count { get; set; }
        public List<string> CampaignIds { get; set; }
    }
}
