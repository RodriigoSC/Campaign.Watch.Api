using System.ComponentModel;


namespace Campaign.Watch.Domain.Enums.Alerts
{
    public enum AlertChannelType
    {
        [Description("E-mail")]
        Email,

        [Description("Webhook")]
        Webhook
    }
}
