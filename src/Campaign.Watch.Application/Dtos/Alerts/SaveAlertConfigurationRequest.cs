using System.ComponentModel.DataAnnotations;

namespace Campaign.Watch.Application.Dtos.Alerts
{
    public class SaveAlertConfigurationRequest
    {
        public string? ClientId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O tipo (canal) é obrigatório.")]
        public string Type { get; set; }

        public string? ConditionType { get; set; }

        public string? MinSeverity { get; set; }

        [Required(ErrorMessage = "O destinatário é obrigatório.")]
        public string Recipient { get; set; }

        public bool IsActive { get; set; }
    }
}
