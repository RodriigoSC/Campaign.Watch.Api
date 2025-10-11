using Campaign.Watch.Infra.Data.Factories.Common;
using MongoDB.Driver;

namespace Campaign.Watch.Infra.Campaign.Factories
{
    /// <summary>
    /// Implementação da fábrica para criar conexões com os bancos de dados de campanha dos clientes.
    /// Atua como um wrapper especializado para a fábrica genérica de MongoDB,
    /// utilizando uma chave de conexão específica para os bancos de dados de campanha.
    /// </summary>
    public class CampaignMongoFactory : ICampaignMongoFactory
    {
        /// <summary>
        /// A fábrica genérica de MongoDB usada para criar a conexão real.
        /// </summary>
        private readonly IMongoDbFactory _factory;

        /// <summary>
        /// Inicializa uma nova instância da classe CampaignMongoFactory.
        /// </summary>
        /// <param name="factory">A fábrica genérica de MongoDB a ser injetada.</param>
        public CampaignMongoFactory(IMongoDbFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Obtém uma instância do banco de dados de campanha de um cliente específico.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente a ser acessado.</param>
        /// <returns>Uma instância de IMongoDatabase configurada para o banco de dados especificado.</returns>
        public IMongoDatabase GetDatabase(string dbName)
        {
            // Utiliza a chave de conexão "MongoDB.Campaign" para obter a conexão correta do pool.
            return _factory.GetDatabase("MongoDB.Campaign", dbName);
        }
    }
}