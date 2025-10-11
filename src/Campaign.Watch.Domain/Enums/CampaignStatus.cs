using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums
{
    /// <summary>
    /// Define os possíveis status de uma campanha, refletindo seu estado no sistema de origem.
    /// </summary>
    public enum CampaignStatus
    {
        /// <summary>
        /// A campanha está em modo de rascunho e ainda não foi ativada.
        /// </summary>
        [Description("Rascunho")] 
        Draft = 0,

        /// <summary>
        /// A campanha foi concluída com sucesso.
        /// </summary>
        [Description("Concluída")]         
        Completed = 1,

        /// <summary>
        /// A campanha encontrou um erro e foi interrompida.
        /// </summary>
        [Description("Erro")] 
        Error = 3,

        /// <summary>
        /// A campanha está atualmente em execução.
        /// </summary>
        [Description("Em Execução")] 
        Executing = 5,

        /// <summary>
        /// A campanha está agendada para ser executada em uma data futura.
        /// </summary>
        [Description("Agendada")] 
        Scheduled = 7,

        /// <summary>
        /// A campanha foi cancelada permanentemente.
        /// </summary>
        [Description("Cancelada")] 
        Canceled = 8,

        /// <summary>
        /// A campanha está no processo de ser cancelada.
        /// </summary>
        [Description("Cancelando")] 
        Canceling = 9
    }
}