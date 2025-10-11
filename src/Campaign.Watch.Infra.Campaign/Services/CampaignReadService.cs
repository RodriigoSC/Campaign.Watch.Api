using Campaign.Watch.Domain.Entities.Read.Campaign;
using Campaign.Watch.Domain.Interfaces.Services.Read;
using Campaign.Watch.Domain.Interfaces.Services.Read.Campaign;
using Campaign.Watch.Infra.Campaign.Factories;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Campaign.Services
{
    /// <summary>
    /// Implementação do serviço para leitura de dados de campanha de fontes de dados externas (bancos de dados dos clientes).
    /// </summary>
    public class CampaignReadService : ICampaignReadService
    {
        /// <summary>
        /// A fábrica para obter conexões com os bancos de dados de campanha dos clientes.
        /// </summary>
        private readonly ICampaignMongoFactory _factory;

        /// <summary>
        /// Inicializa uma nova instância da classe CampaignReadService.
        /// </summary>
        /// <param name="factory">A fábrica de conexão com os bancos de dados de campanha a ser injetada.</param>
        public CampaignReadService(ICampaignMongoFactory factory)
        {
            _factory = factory;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CampaignRead>> GetCampaignsByClient(string dbName)
        {
            var db = _factory.GetDatabase(dbName);
            var collection = db.GetCollection<CampaignRead>("Campaign");
            return await collection.Find(_ => true).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ExecutionRead>> GetExecutionsByCampaign(string dbName, string campaignId)
        {
            var db = _factory.GetDatabase(dbName);
            var collection = db.GetCollection<ExecutionRead>("ExecutionPlan");

            return await collection
                .Find(x => x.CampaignId.ToString() == campaignId && x.FlagCount == false)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<CampaignRead> GetCampaignById(string dbName, string campaignId)
        {
            var db = _factory.GetDatabase(dbName);
            var collection = db.GetCollection<CampaignRead>("Campaign");

            if (!ObjectId.TryParse(campaignId, out var campaignObjectId))
            {
                return null;
            }

            return await collection.Find(x => x.Id == campaignObjectId.ToString()).FirstOrDefaultAsync();
        }
    }
}