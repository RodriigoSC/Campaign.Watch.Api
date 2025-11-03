using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums.Alerts
{
    public enum AlertSeverity
    {
        [Description("Healthy")]
        Healthy,

        [Description("Warning")]
        Warning,

        [Description("Error")]
        Error,

        [Description("Critical")]
        Critical
    }
}
