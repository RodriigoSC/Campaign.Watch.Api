using Campaign.Watch.Domain.Entities.Alerts;
using Campaign.Watch.Domain.Interfaces.Repositories.Alerts;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Repository.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Repository.Alerts
{
    public class AlertHistoryRepository : CommonRepository<AlertHistoryEntity>, IAlertHistoryRepository
    {
        public AlertHistoryRepository(IPersistenceMongoFactory persistenceFactory)
            : base(persistenceFactory, "AlertHistory")
        {
            var indexKeys = Builders<AlertHistoryEntity>.IndexKeys
                .Ascending(x => x.ClientId)
                .Descending(x => x.DetectedAt);

            var indexModel = new CreateIndexModel<AlertHistoryEntity>(
                indexKeys,
                new CreateIndexOptions { Name = "AlertHistory_Scope_Query" });

            CreateIndexesAsync(new List<CreateIndexModel<AlertHistoryEntity>> { indexModel }).GetAwaiter().GetResult();
        }

        public async Task<AlertHistoryEntity> CreateAsync(AlertHistoryEntity entity)
        {
            // O worker será responsável por preencher o DetectedAt
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<AlertHistoryEntity>> GetByScopeAsync(ObjectId? clientId, CancellationToken cancellationToken, int limit = 100)
        {
            var filter = Builders<AlertHistoryEntity>.Filter.Eq(e => e.ClientId, clientId);
            return await _collection.Find(filter)
                .SortByDescending(e => e.DetectedAt)
                .Limit(limit)
                .ToListAsync(cancellationToken);
        }
    }
}
