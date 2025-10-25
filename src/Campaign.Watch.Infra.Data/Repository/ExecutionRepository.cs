using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Repository.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Repository
{
    public class ExecutionRepository : CommonRepository<ExecutionEntity>, IExecutionRepository
    {
        public ExecutionRepository(IPersistenceMongoFactory persistenceFactory)
            : base(persistenceFactory, "ExecutionMonitoring")
        {
            // Criar índices
            var uniqueIndexKeys = Builders<ExecutionEntity>.IndexKeys
                .Ascending(x => x.OriginalCampaignId)
                .Ascending(x => x.OriginalExecutionId);

            var uniqueIndexModel = new CreateIndexModel<ExecutionEntity>(
                uniqueIndexKeys,
                new CreateIndexOptions { Unique = true, Name = "OriginalCampaign_OriginalExecution_Unique" });

            var campaignIndexKeys = Builders<ExecutionEntity>.IndexKeys
                .Ascending(x => x.CampaignMonitoringId);

            var campaignIndexModel = new CreateIndexModel<ExecutionEntity>(
                campaignIndexKeys,
                new CreateIndexOptions { Name = "CampaignMonitoringId_Query" });

            CreateIndexesAsync(new List<CreateIndexModel<ExecutionEntity>>
            {
                uniqueIndexModel,
                campaignIndexModel
            }).GetAwaiter().GetResult();
        }

        public async Task<ExecutionEntity> ObterExecucaoPorIdAsync(string originalExecutionId)
        {
            return await _collection
                .Find(e => e.OriginalExecutionId == originalExecutionId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExecutionEntity>> ObterExecucoesPorCampanhaAsync(
            ObjectId campaignMonitoringId)
        {
            return await _collection
                .Find(e => e.CampaignMonitoringId == campaignMonitoringId)
                .SortByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExecutionEntity>> ObterExecucoesComErrosAsync(
            string clientName = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            var filterBuilder = Builders<ExecutionEntity>.Filter;
            var filter = filterBuilder.Eq(e => e.HasMonitoringErrors, true);

            if (dataInicio.HasValue)
            {
                filter &= filterBuilder.Gte(e => e.StartDate, dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                filter &= filterBuilder.Lte(e => e.StartDate, dataFim.Value);
            }

            // Se precisar filtrar por cliente, terá que fazer join ou lookup
            // Por simplicidade, retornando todas com erro no período

            return await _collection
                .Find(filter)
                .SortByDescending(e => e.StartDate)
                .ToListAsync();
        }
    }
}