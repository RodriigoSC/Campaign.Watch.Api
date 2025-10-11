using System.ComponentModel;

namespace Campaign.Watch.Domain.Enums
{
    public enum MonitoringStatus
    {
        /// <summary>
        /// A campanha está agendada mas ainda não teve sua primeira execução.
        /// </summary>
        [Description("Pendente")] 
        Pending,

        /// <summary>
        /// A campanha está em andamento. O worker está ativamente monitorando seus steps.
        /// Válido para campanhas pontuais com pausas internas.
        /// </summary>
        [Description("Em andamento")] 
        InProgress,

        /// <summary>
        /// A campanha concluiu sua última execução com sucesso e agora aguarda o próximo ciclo.
        /// (Específico para campanhas recorrentes).
        /// </summary>
        [Description("Aguardando próxima execução")] 
        WaitingForNextExecution,

        /// <summary>
        /// A campanha (sendo pontual) foi concluída com sucesso e não será mais monitorada.
        /// </summary>
        [Description("Concluído")] 
        Completed,

        /// <summary>
        /// Ocorreu um erro na última tentativa de execução que precisa de atenção.
        /// </summary>
        [Description("Falha")] 
        Failed,

        /// <summary>
        /// A campanha já deveria ter iniciado sua execução, mas não há registros de que isso aconteceu.
        /// </summary>
        [Description("Execução atrasada")] 
        ExecutionDelayed
    }
}
