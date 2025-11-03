using Campaign.Watch.Application.Dtos.Common;

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

        // DTOs aninhados removidos e importados
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }
}