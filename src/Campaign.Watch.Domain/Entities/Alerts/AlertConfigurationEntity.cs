using Campaign.Watch.Domain.Entities.Common;
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

        public string Type { get; set; }

        public string ConditionType { get; set; }

        public string MinSeverity { get; set; }

        public string Recipient { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
