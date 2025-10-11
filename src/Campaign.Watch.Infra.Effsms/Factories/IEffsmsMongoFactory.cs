using MongoDB.Driver;

namespace Campaign.Watch.Infra.Effsms.Factories
{
    public interface IEffsmsMongoFactory
    {
        IMongoDatabase GetDatabase(string dbName);
    }
}
