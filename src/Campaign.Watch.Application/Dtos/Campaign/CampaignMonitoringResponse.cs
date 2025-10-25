using System;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    public class CampaignMonitoringResponse
    {
        public string Id { get; set; }
        public string ClientName { get; set; }
        public string IdCampaign { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string CampaignType { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CampaignStatus { get; set; }
        public string MonitoringStatus { get; set; }
        public DateTime? NextExecutionMonitoring { get; set; }
        public DateTime? LastCheckMonitoring { get; set; }
        public MonitoringHealthStatusDto HealthStatus { get; set; }
        public SchedulerResponse Scheduler { get; set; }

        // Campos do worker que estavam faltando
        public int TotalExecutionsProcessed { get; set; }
        public int ExecutionsWithErrors { get; set; }
        public DateTime? FirstMonitoringAt { get; set; }
        public string MonitoringNotes { get; set; }

        // Métricas calculadas (opcional - pode ser removido se preferir endpoint separado)
        public CampaignMetricsDto Metrics { get; set; }
    }

    /// <summary>
    /// DTO para agrupar as flags de saúde e problemas do monitoramento.
    /// </summary>
    public class MonitoringHealthStatusDto
    {
        public bool IsFullyVerified { get; set; }
        public bool HasPendingExecution { get; set; }
        public bool HasIntegrationErrors { get; set; }
        public string LastExecutionWithIssueId { get; set; }
        public string LastMessage { get; set; }
    }

    public class SchedulerResponse
    {
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsRecurrent { get; set; }
        public string Crontab { get; set; }
    }
}