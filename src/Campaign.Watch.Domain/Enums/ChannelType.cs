using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums
{
    /// <summary>
    /// Define os tipos de canais de comunicação disponíveis.
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// Canal de E-mail.
        /// </summary>
        [Description("EffectiveMail")] 
        EffectiveMail = 1,

        /// <summary>
        /// Canal de SMS (Short Message Service).
        /// </summary>
        [Description("EffectiveSms")] 
        EffectiveSms = 2,

        /// <summary>
        /// Canal de  Push.
        /// </summary>
        [Description("EffectivePush")] 
        EffectivePush = 3,

        /// <summary>
        /// Canal de WhatsApp.
        /// </summary>
        [Description("EffectiveWhatsApp")] 
        EffectiveWhatsApp = 5,

        /// <summary>
        /// Canal de API.
        /// </summary>       
        [Description("EffectiveApi")]
        EffectiveApi = 6
    }
}