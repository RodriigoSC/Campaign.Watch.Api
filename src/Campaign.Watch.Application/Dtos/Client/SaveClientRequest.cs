using Campaign.Watch.Domain.Enums;
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

        public List<EffectiveChannelDto> EffectiveChannels { get; set; }
    }

    public class CampaignConfigDto
    {
        [Required]
        public string ProjectID { get; set; }
        public string Database { get; set; }
    }

    public class EffectiveChannelDto
    {
        public ChannelType TypeChannel { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Database { get; set; }
        public string TenantID { get; set; }
    }
}