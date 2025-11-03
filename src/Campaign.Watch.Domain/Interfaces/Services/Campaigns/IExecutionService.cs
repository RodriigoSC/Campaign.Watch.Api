using Campaign.Watch.Domain.Entities.Campaign;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Campaign
{
    public interface IExecutionService
    {
        Task<ExecutionEntity> ObterExecucaoPorIdAsync(string originalExecutionId);
        Task<IEnumerable<ExecutionEntity>> ObterExecucoesPorCampanhaAsync(ObjectId campaignMonitoringId);
        Task<IEnumerable<ExecutionEntity>> ObterExecucoesComErrosAsync(string clientName = null,DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
