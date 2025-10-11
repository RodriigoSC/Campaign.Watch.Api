using MongoDB.Driver;

namespace Campaign.Watch.Infra.Effmail.Factories
{
    public interface IEffmailMongoFactory
    {
        IMongoDatabase GetDatabase(string dbName);
    }
}
