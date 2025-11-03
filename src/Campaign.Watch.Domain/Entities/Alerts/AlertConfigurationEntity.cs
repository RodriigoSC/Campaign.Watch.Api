using Campaign.Watch.Domain.Entities.Common;
using Campaign.Watch.Domain.Enums.Alerts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Campaign.Watch.Domain.Entities.Alerts
{
    [BsonIgnoreExtraElements]
    public class AlertConfigurationEntity : CommonEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ClientId { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.String)]
        public AlertChannelType Type { get; set; }

        [BsonRepresentation(BsonType.String)]
        public AlertConditionType? ConditionType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public AlertSeverity? MinSeverity { get; set; }

        public string Recipient { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
