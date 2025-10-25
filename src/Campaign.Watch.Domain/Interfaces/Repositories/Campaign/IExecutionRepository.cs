using Campaign.Watch.Domain.Entities.Campaign;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Repositories.Campaign
{
    public interface IExecutionRepository
    {
        Task<ExecutionEntity> ObterExecucaoPorIdAsync(string originalExecutionId);
        Task<IEnumerable<ExecutionEntity>> ObterExecucoesPorCampanhaAsync(ObjectId campaignMonitoringId);
        Task<IEnumerable<ExecutionEntity>> ObterExecucoesComErrosAsync(string clientName = null, System.DateTime? dataInicio = null, System.DateTime? dataFim = null);
    }
}
