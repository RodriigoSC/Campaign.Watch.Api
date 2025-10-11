using System;
using System.Collections.Generic;

namespace Campaign.Watch.Application.Dtos.Client
{
    public class ClientResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public CampaignConfigDto CampaignConfig { get; set; }
        public List<EffectiveChannelDto> EffectiveChannels { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
