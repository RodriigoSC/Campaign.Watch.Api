using Campaign.Watch.Domain.Entities.Client;
using Campaign.Watch.Domain.Interfaces.Repositories.Client;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Repository.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Repository
{
    /// <summary>
    /// Repositório para gerenciar as operações de dados da entidade ClientEntity.
    /// </summary>
    public class ClientRepository : CommonRepository<ClientEntity>, IClientRepository
    {
        /// <summary>
        /// Inicializa uma nova instância da classe ClientRepository.
        /// Configura a coleção "Clients" e garante a criação de um índice único para o nome do cliente.
        /// </summary>
        /// <param name="persistenceFactory">A fábrica para obter a instância do banco de dados.</param>
        public ClientRepository(IPersistenceMongoFactory persistenceFactory) : base(persistenceFactory, "Clients")
        {
            var indexKeys = Builders<ClientEntity>.IndexKeys.Ascending(x => x.Name);
            var indexModel = new CreateIndexModel<ClientEntity>(
                indexKeys,
                new CreateIndexOptions { Unique = true }
            );

            // Garante que o índice seja criado antes de qualquer operação.
            CreateIndexesAsync(new List<CreateIndexModel<ClientEntity>> { indexModel }).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task<ClientEntity> CreateAsync(ClientEntity client)
        {
            await _collection.InsertOneAsync(client);
            return client;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClientEntity>> GetAllAsync()
        {
            return await _collection.Find(Builders<ClientEntity>.Filter.Empty).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<ClientEntity> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<ClientEntity> GetByNameAsync(string clientName)
        {
            return await _collection.Find(c => c.Name == clientName).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Este método atualiza explicitamente a propriedade ModifiedAt para a data e hora atuais (UTC).
        /// </remarks>
        public async Task<bool> UpdateAsync(ObjectId id, ClientEntity client)
        {
            var updateDefinition = Builders<ClientEntity>.Update
                .Set(c => c.Name, client.Name)
                .Set(c => c.IsActive, client.IsActive)
                .Set(c => c.CampaignConfig, client.CampaignConfig)
                .Set(c => c.EffectiveChannels, client.EffectiveChannels)
                .Set(c => c.ModifiedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(c => c.Id == id, updateDefinition);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _collection.DeleteOneAsync(c => c.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}