using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Services
{
    public class ExecutionService : IExecutionService
    {
        private readonly IExecutionRepository _executionRepository;

        public ExecutionService(IExecutionRepository executionRepository)
        {
            _executionRepository = executionRepository;
        }

        public async Task<ExecutionEntity> ObterExecucaoPorIdAsync(string originalExecutionId)
        {
            return await _executionRepository.ObterExecucaoPorIdAsync(originalExecutionId);
        }

        public async Task<IEnumerable<ExecutionEntity>> ObterExecucoesPorCampanhaAsync(
            ObjectId campaignMonitoringId)
        {
            return await _executionRepository.ObterExecucoesPorCampanhaAsync(campaignMonitoringId);
        }

        public async Task<IEnumerable<ExecutionEntity>> ObterExecucoesComErrosAsync(
            string clientName = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            return await _executionRepository.ObterExecucoesComErrosAsync(
                clientName, dataInicio, dataFim);
        }
    }
}
