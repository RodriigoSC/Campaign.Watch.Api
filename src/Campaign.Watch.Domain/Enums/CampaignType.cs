using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums
{
    /// <summary>
    /// Define o tipo de execução de uma campanha.
    /// </summary>
    public enum CampaignType
    {
        /// <summary>
        /// A campanha é executada apenas uma vez.
        /// </summary>
        [Description("Pontual")] 
        Pontual = 0,

        /// <summary>
        /// A campanha é executada repetidamente, com base em um agendamento.
        /// </summary>
        [Description("Recorrente")] 
        Recorrente = 1
    }
}