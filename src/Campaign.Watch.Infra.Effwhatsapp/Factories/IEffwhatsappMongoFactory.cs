using MongoDB.Driver;

namespace Campaign.Watch.Infra.Effwhatsapp.Factories
{
    public interface IEffwhatsappMongoFactory
    {
        IMongoDatabase GetDatabase(string dbName);
    }
}
