using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Effpush.Factories
{
    public interface IEffpushMongoFactory
    {
        IMongoDatabase GetDatabase(string dbName);
    }
}
