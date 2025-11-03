using System.ComponentModel.DataAnnotations;

namespace Campaign.Watch.Application.Dtos.Client
{
    public class SaveClientRequest
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "A configuração do campaign é obrigatória.")]
        public CampaignConfigDto CampaignConfig { get; set; }
    }

    public class CampaignConfigDto
    {
        [Required(ErrorMessage = "O ProjectID é obrigatório.")]
        public string ProjectID { get; set; }
        public string Database { get; set; }
    }    
}