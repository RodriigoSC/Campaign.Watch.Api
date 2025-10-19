using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO para dados de integração de canal
    /// </summary>
    public class ChannelIntegrationDataResponse
    {
        public string ChannelName { get; set; }
        public string IntegrationStatus { get; set; }
        public string TemplateId { get; set; }
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }
}
