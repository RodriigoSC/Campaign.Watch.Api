using MongoDB.Driver;

namespace Campaign.Watch.Infra.Data.Factories
{
    /// <summary>
    /// Define uma interface para a fábrica (factory) responsável por fornecer acesso
    /// ao banco de dados principal de persistência da aplicação.
    /// </summary>
    public interface IPersistenceMongoFactory
    {
        /// <summary>
        /// Obtém a instância do banco de dados principal de persistência.
        /// </summary>
        /// <returns>Uma instância de IMongoDatabase representando o banco de dados da aplicação.</returns>
        IMongoDatabase GetDatabase();
    }
}