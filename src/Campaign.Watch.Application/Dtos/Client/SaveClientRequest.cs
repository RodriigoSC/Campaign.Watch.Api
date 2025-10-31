using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Campaign.Watch.Application.Dtos.Client
{
    public class SaveClientRequest
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public CampaignConfigDto CampaignConfig { get; set; }

        public EffectiveChannelsDto EffectiveChannels { get; set; }
    }

    public class CampaignConfigDto
    {
        [Required]
        public string ProjectID { get; set; }
        public string Database { get; set; }
    }

    public class EffectiveChannelsDto
    {
        public List<ChannelDbConfigDto> EFFMAIL { get; set; } = new();
        public List<ChannelDbConfigDto> EFFSMS { get; set; } = new();
        public List<ChannelDbConfigDto> EFFPUSH { get; set; } = new();
        public List<ChannelDbConfigDto> EFFWHATSAPP { get; set; } = new();
    }

    public class ChannelDbConfigDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Integration { get; set; }
        [Required]
        public string DataBase { get; set; }
    }
}