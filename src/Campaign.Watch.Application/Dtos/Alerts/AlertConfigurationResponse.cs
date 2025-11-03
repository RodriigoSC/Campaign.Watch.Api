using System;

namespace Campaign.Watch.Application.Dtos.Alerts
{
    public class AlertConfigurationResponse
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ConditionType { get; set; }
        public string MinSeverity { get; set; }
        public string Recipient { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
