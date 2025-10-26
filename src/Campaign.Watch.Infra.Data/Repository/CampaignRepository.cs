using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Repository.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Repository
{
    public class CampaignRepository : CommonRepository<CampaignEntity>, ICampaignRepository
    {
        public CampaignRepository(IPersistenceMongoFactory persistenceFactory) : base(persistenceFactory, "CampaignMonitoring")
        {
            var uniqueIndexKeys = Builders<CampaignEntity>.IndexKeys
                .Ascending(x => x.ClientName)
                .Ascending(x => x.IdCampaign);
            var uniqueIndexModel = new CreateIndexModel<CampaignEntity>(
                uniqueIndexKeys,
                new CreateIndexOptions { Unique = true, Name = "Client_IdCampaign_Unique" });


            var workerIndexKeys = Builders<CampaignEntity>.IndexKeys
                .Ascending(x => x.IsActive)
                .Ascending(x => x.NextExecutionMonitoring);
            var workerIndexModel = new CreateIndexModel<CampaignEntity>(
                workerIndexKeys,
                new CreateIndexOptions { Name = "Worker_Monitoring_Query" });

            CreateIndexesAsync(new List<CreateIndexModel<CampaignEntity>> { uniqueIndexModel, workerIndexModel }).GetAwaiter().GetResult();
        }

        public async Task<CampaignEntity> CriarCampanhaAsync(CampaignEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> AtualizarCampanhaAsync(ObjectId id, CampaignEntity entity)
        {
            var filter = Builders<CampaignEntity>.Filter.Eq(c => c.IdCampaign, entity.IdCampaign);
            var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = true });
            return result.IsAcknowledged && (result.ModifiedCount > 0 || result.UpsertedId != null);
        }

        public async Task<CampaignEntity> ObterCampanhaPorIdAsync(ObjectId id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasAsync()
        {
            return await _collection.Find(FilterDefinition<CampaignEntity>.Empty).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorClienteAsync(string nomeCliente)
        {
            return await _collection.Find(c => c.ClientName == nomeCliente).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorDataAsync(DateTime inicio, DateTime fim)
        {
            var filter = Builders<CampaignEntity>.Filter.Gte(c => c.CreatedAt, inicio) &
                         Builders<CampaignEntity>.Filter.Lte(c => c.CreatedAt, fim);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorClienteOuDataAsync(string nomeCliente, DateTime inicio, DateTime fim)
        {
            var filter = Builders<CampaignEntity>.Filter.And(
                Builders<CampaignEntity>.Filter.Eq(c => c.ClientName, nomeCliente),
                Builders<CampaignEntity>.Filter.Gte(c => c.CreatedAt, inicio),
                Builders<CampaignEntity>.Filter.Lte(c => c.CreatedAt, fim)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasAtivasAsync()
        {
            return await _collection.Find(c => c.IsActive == true).ToListAsync();
        }

        public async Task<int> ContarCampanhasPorClienteAsync(string nomeCliente)
        {
            return (int)await _collection.CountDocumentsAsync(c => c.ClientName == nomeCliente);
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasPaginadasAsync(int pagina, int tamanhoPagina)
        {
            return await _collection.Find(FilterDefinition<CampaignEntity>.Empty)
                                      .Skip((pagina - 1) * tamanhoPagina)
                                      .Limit(tamanhoPagina)
                                      .ToListAsync();
        }

        public async Task<CampaignEntity> ObterCampanhaPorNomeAsync(string nomeCampanha)
        {
            return await _collection.Find(c => c.Name == nomeCampanha).FirstOrDefaultAsync();
        }

        public async Task<CampaignEntity> ObterCampanhaPorNumeroAsync(long numeroCampanha)
        {
            return await _collection.Find(c => c.NumberId == numeroCampanha).FirstOrDefaultAsync();
        }

        public async Task<CampaignEntity> ObterCampanhaPorIdCampanhaAsync(string nomeCliente, string idCampanha)
        {
            var filter = Builders<CampaignEntity>.Filter.And(
                Builders<CampaignEntity>.Filter.Eq(c => c.ClientName, nomeCliente),
                Builders<CampaignEntity>.Filter.Eq(c => c.IdCampaign, idCampanha)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasParaMonitorarAsync()
        {
            var now = DateTime.UtcNow;

            var filter = Builders<CampaignEntity>.Filter.And(
                Builders<CampaignEntity>.Filter.Eq(c => c.IsActive, true),
                Builders<CampaignEntity>.Filter.Lte(c => c.NextExecutionMonitoring, now)
            );

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasComErrosDeIntegracaoAsync()
        {
            return await _collection.Find(c => c.HealthStatus.HasIntegrationErrors == true).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasComExecucaoAtrasadaAsync()
        {
            return await _collection.Find(c => c.MonitoringStatus == MonitoringStatus.ExecutionDelayed).ToListAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasMonitoradasComSucessoAsync()
        {
            return await _collection.Find(c => c.MonitoringStatus == MonitoringStatus.Completed || c.MonitoringStatus == MonitoringStatus.WaitingForNextExecution).ToListAsync();
        }     
    }
}