using Campaign.Watch.Domain.Entities.Read.Effmail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Read.Effmail
{
    /// <summary>
    /// Define a interface para o serviço de leitura de dados de Effmail de uma fonte externa.
    /// </summary>
    public interface IEffmailReadService
    {
        /// <summary>
        /// Busca os dados da trigger do Effmail associados a um workflow específico.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente a ser consultado.</param>
        /// <param name="workflowId">O ID do workflow para o qual os dados da trigger serão buscados.</param>
        /// <returns>Uma coleção de dados de Effmail lidos da fonte de dados.</returns>
        Task<IEnumerable<EffmailRead>> GetTriggerEffmail(string dbName, string workflowId);
    }
}