using Campaign.Watch.Domain.Entities.Common;
using Campaign.Watch.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Domain.Entities.Campaign
{
    /// <summary>
    /// Representa a entidade principal de uma campanha no sistema de monitoramento.
    /// ATUALIZADA para corresponder ao CampaignModel do Worker.
    /// </summary>
    public class CampaignEntity : CommonEntity
    {
        public string ClientName { get; set; }
        public string IdCampaign { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public CampaignType CampaignType { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public CampaignStatus StatusCampaign { get; set; }
        public MonitoringStatus MonitoringStatus { get; set; }
        public DateTime? NextExecutionMonitoring { get; set; }
        public DateTime? LastCheckMonitoring { get; set; }
        public MonitoringHealthStatus HealthStatus { get; set; }
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsDeleted { get; set; }
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsRestored { get; set; }
        public Scheduler Scheduler { get; set; }
        public int TotalExecutionsProcessed { get; set; }
        public int ExecutionsWithErrors { get; set; }
        public DateTime? FirstMonitoringAt { get; set; }
        public string MonitoringNotes { get; set; }        
    }

    public class MonitoringHealthStatus
    {
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsFullyVerified { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool HasPendingExecution { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool HasIntegrationErrors { get; set; }

        public string LastExecutionWithIssueId { get; set; }
        public string LastMessage { get; set; }
    }

    public class Scheduler
    {
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsRecurrent { get; set; }

        public string Crontab { get; set; }
    }   
}