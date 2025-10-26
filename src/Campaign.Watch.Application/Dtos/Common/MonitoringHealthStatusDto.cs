namespace Campaign.Watch.Application.Dtos.Common
{
    public class MonitoringHealthStatusDto
    {
        public bool IsFullyVerified { get; set; }
        public bool HasPendingExecution { get; set; }
        public bool HasIntegrationErrors { get; set; }
        public string LastExecutionWithIssueId { get; set; }
        public string LastMessage { get; set; }
    }
}
