using Campaign.Watch.Domain.Entities.Read.Effwhatsapp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Read.Effwhatsapp
{
    public interface IEffwhatsappReadService
    {
        /// <summary>
        /// Busca os dados da trigger do Effwhatsapp associados a um workflow específico,
        /// agregando as estatísticas dos leads.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente a ser consultado.</param>
        /// <param name="workflowId">O ID do workflow para o qual os dados da trigger serão buscados.</param>
        /// <returns>Uma coleção de dados de Effwhatsapp lidos e agregados da fonte de dados.</returns>
        Task<IEnumerable<EffwhatsappRead>> GetTriggerEffwhatsapp(string dbName, string workflowId);
    }
}
