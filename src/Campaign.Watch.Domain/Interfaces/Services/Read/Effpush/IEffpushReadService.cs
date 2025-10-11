using Campaign.Watch.Domain.Entities.Read.Effpush;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Read.Effpush
{
    public interface IEffpushReadService
    {
        /// <summary>
        /// Busca os dados da trigger do Effpush associados a um workflow específico,
        /// agregando as estatísticas dos leads.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente a ser consultado.</param>
        /// <param name="workflowId">O ID do workflow para o qual os dados da trigger serão buscados.</param>
        /// <returns>Uma coleção de dados de Effpush lidos e agregados da fonte de dados.</returns>
        Task<IEnumerable<EffpushRead>> GetTriggerEffpush(string dbName, string workflowId);
    }
}
