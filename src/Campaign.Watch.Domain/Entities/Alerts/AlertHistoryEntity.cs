using Campaign.Watch.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Campaign.Watch.Domain.Entities.Alerts
{
    [BsonIgnoreExtraElements]
    public class AlertHistoryEntity : CommonEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ClientId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId AlertConfigurationId { get; set; }

        public string Severity { get; set; }

        public string Message { get; set; }

        public string CampaignName { get; set; }

        public string StepName { get; set; }

        public DateTime DetectedAt { get; set; }
    }
}
