using System;

namespace Campaign.Watch.Application.Dtos.Alerts
{
    public class AlertHistoryResponse
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string AlertConfigurationId { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public string CampaignName { get; set; }
        public string StepName { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
