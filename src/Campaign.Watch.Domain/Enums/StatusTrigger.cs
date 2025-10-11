using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums
{
    public enum StatusTrigger
    {
        [Description("Concluído")] 
        Concluded,

        [Description("Agendado")] 
        Scheduler,

        [Description("Erro")] 
        Error
    }
}
