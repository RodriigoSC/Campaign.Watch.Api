using Campaign.Watch.Domain.Entities.Alerts;
using Campaign.Watch.Domain.Interfaces.Repositories.Alerts;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Repository.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Repository.Alerts
{
    public class AlertConfigurationRepository : CommonRepository<AlertConfigurationEntity>, IAlertConfigurationRepository
    {
        public AlertConfigurationRepository(IPersistenceMongoFactory persistenceFactory)
            : base(persistenceFactory, "AlertConfiguration")
        {
            var indexKeys = Builders<AlertConfigurationEntity>.IndexKeys
                .Ascending(x => x.ClientId);

            var indexModel = new CreateIndexModel<AlertConfigurationEntity>(
                indexKeys,
                new CreateIndexOptions { Name = "Alerts_ClientId_Scope_Query" });

            CreateIndexesAsync(new List<CreateIndexModel<AlertConfigurationEntity>> { indexModel }).GetAwaiter().GetResult();
        }

        public async Task<AlertConfigurationEntity> CreateAsync(AlertConfigurationEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _collection.DeleteOneAsync(e => e.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<AlertConfigurationEntity> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AlertConfigurationEntity>> GetByScopeAsync(ObjectId? clientId, CancellationToken cancellationToken)
        {
            var filter = Builders<AlertConfigurationEntity>.Filter.Eq(e => e.ClientId, clientId);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(ObjectId id, AlertConfigurationEntity entity)
        {
            var updateDefinition = Builders<AlertConfigurationEntity>.Update
                .Set(e => e.Name, entity.Name)
                .Set(e => e.Type, entity.Type)
                .Set(e => e.ConditionType, entity.ConditionType)
                .Set(e => e.MinSeverity, entity.MinSeverity)
                .Set(e => e.Recipient, entity.Recipient)
                .Set(e => e.IsActive, entity.IsActive);

            var result = await _collection.UpdateOneAsync(e => e.Id == id, updateDefinition);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
