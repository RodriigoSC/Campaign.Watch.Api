using System;

namespace Campaign.Watch.Application.Dtos.Client
{
    public class ClientResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public CampaignConfigDto CampaignConfig { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}