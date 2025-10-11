using Campaign.Watch.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    public class CampaignDetailResponse
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
        public List<ExecutionResponse> Executions { get; set; }
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

    public class ExecutionResponse
    {
        public string ExecutionId { get; set; }
        public string CampaignName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double StartLatencyInSeconds { get; set; }
        public double? TotalDurationInSeconds { get; set; }
        public bool IsFullyVerifiedByMonitoring { get; set; }
        public bool HasMonitoringErrors { get; set; }
        public List<WorkflowResponse> Steps { get; set; }
    }

    public class WorkflowResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ChannelName { get; set; }
        public string Status { get; set; }
        public long TotalUser { get; set; }
        public long TotalExecutionTime { get; set; }
        public object Error { get; set; }

        /// <summary>
        /// Armazena notas ou mensagens de erro específicas do worker para este step.
        /// </summary>
        public string MonitoringNotes { get; set; }

        public IntegrationDataResponseBase IntegrationData { get; set; }
    }

    public abstract class IntegrationDataResponseBase
    {
        public string ChannelName { get; set; }
        public string IntegrationStatus { get; set; }
    }

    public class EmailIntegrationDataResponse : IntegrationDataResponseBase
    {
        public string TemplateId { get; set; }
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }

    public class SmsIntegrationDataResponse : IntegrationDataResponseBase
    {
        public string TemplateId { get; set; }
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }

    public class PushIntegrationDataResponse : IntegrationDataResponseBase
    {
        public string TemplateId { get; set; }
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }

    public class WhatsAppIntegrationDataResponse : IntegrationDataResponseBase
    {
        public string TemplateId { get; set; }
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }

    public class FileInfoDataResponse
    {
        public string Name { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public long Total { get; set; }
    }

    public class LeadsDataResponse
    {
        public int? Blocked { get; set; }
        public int? Deduplication { get; set; }
        public int? Error { get; set; }
        public int? Optout { get; set; }
        public int? Success { get; set; }
    }
}