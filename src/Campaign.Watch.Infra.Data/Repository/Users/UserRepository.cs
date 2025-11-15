using Campaign.Watch.Domain.Entities.Users;
using Campaign.Watch.Domain.Interfaces.Repositories.Users;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Repository.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Repository.Users
{
    public class UserRepository : CommonRepository<UserEntity>, IUserRepository
    {
        public UserRepository(IPersistenceMongoFactory persistenceFactory) : base(persistenceFactory, "Users")
        {
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(x => x.Email);
            var indexModel = new CreateIndexModel<UserEntity>(
                indexKeys,
                new CreateIndexOptions { Unique = true }
            );

            CreateIndexesAsync(new List<CreateIndexModel<UserEntity>> { indexModel }).GetAwaiter().GetResult();
        }

        public async Task<UserEntity> CreateAsync(UserEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<UserEntity> GetByEmailAsync(string email)
        {
            return await _collection.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<UserEntity> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(UserEntity entity)
        {
            entity.ModifiedAt = DateTime.UtcNow;

            var result = await _collection.ReplaceOneAsync(u => u.Id == entity.Id, entity);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        public async Task<IEnumerable<UserEntity>> GetAllAsync()
        {
            return await _collection.Find(Builders<UserEntity>.Filter.Empty)
                                    .SortBy(u => u.Name)
                                    .ToListAsync();
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _collection.DeleteOneAsync(u => u.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<long> CountByRoleAsync(string role)
        {
            var filter = Builders<UserEntity>.Filter.Eq(u => u.Role, role);
            return await _collection.CountDocumentsAsync(filter);
        }
    }
}
