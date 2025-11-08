using Campaign.Watch.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Campaign.Watch.Domain.Entities.Client
{
    /// <summary>
    /// Entidade principal do Cliente.
    /// </summary>
    public class ClientEntity : CommonEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public CampaignConfig CampaignConfig { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public ClientEntity()
        {
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateModifiedAt()
        {
            ModifiedAt = DateTime.UtcNow;
        }
    }    
    
    public class CampaignConfig
    {
        public string ProjectID { get; set; }
        public string Database { get; set; }
    }


}