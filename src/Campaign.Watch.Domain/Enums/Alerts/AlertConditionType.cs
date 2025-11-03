using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums.Alerts
{
    public enum AlertConditionType
    {
        [Description("Etapa Falhou")]
        StepFailed,

        [Description("Execução Atrasada")]
        ExecutionDelayed,

        [Description("Filtro Travado")]
        FilterStuck,

        [Description("Erro de Integração")]
        IntegrationError,

        [Description("Campanha Não Finalizada")]
        CampaignNotFinalized
    }
}
