using System;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    public class CampaignStatusResponse
    {
        public string Id { get; set; }
        public string ClientName { get; set; }
        public string IdCampaign { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string CampaignType { get; set; }
        public string Description { get; set; }
        public string CampaignStatus { get; set; }
        public string MonitoringStatus { get; set; }
        public DateTime? NextExecutionMonitoring { get; set; }
        public DateTime? LastCheckMonitoring { get; set; }
        public MonitoringHealthStatusDto HealthStatus { get; set; }

    }
}
