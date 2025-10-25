using Campaign.Watch.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Domain.Entities.Campaign
{
    /// <summary>
    /// Entidade de execução armazenada em collection separada (ExecutionMonitoring).
    /// Corresponde ao ExecutionModel do Worker.
    /// </summary>
    public class ExecutionEntity : CommonEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId CampaignMonitoringId { get; set; }

        public string OriginalCampaignId { get; set; }
        public string OriginalExecutionId { get; set; }
        public string CampaignName { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? TotalDurationInSeconds { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool HasMonitoringErrors { get; set; }

        public List<WorkflowStepEntity> Steps { get; set; }
    }

    /// <summary>
    /// Representa um step dentro de uma execução.
    /// Corresponde ao WorkflowStep do Worker.
    /// </summary>
    public class WorkflowStepEntity
    {
        public string OriginalStepId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public long TotalUser { get; set; }
        public long TotalExecutionTime { get; set; }
        public string Error { get; set; }
        public string ChannelName { get; set; }
        public string MonitoringNotes { get; set; }
        public ChannelIntegrationDataEntity IntegrationData { get; set; }
    }

    /// <summary>
    /// Dados de integração do canal.
    /// Corresponde ao ChannelIntegrationData do Worker.
    /// </summary>
    public class ChannelIntegrationDataEntity
    {
        public string Raw { get; set; }
        public string ChannelName { get; set; }
        public string IntegrationStatus { get; set; }
        public string TemplateId { get; set; }
        public FileInfoDataEntity File { get; set; }
        public LeadsDataEntity Leads { get; set; }
    }

    public class FileInfoDataEntity
    {
        public string Name { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public long Total { get; set; }
    }

    public class LeadsDataEntity
    {
        public int? Blocked { get; set; }
        public int? Deduplication { get; set; }
        public int? Error { get; set; }
        public int? Optout { get; set; }
        public int? Success { get; set; }
    }
}