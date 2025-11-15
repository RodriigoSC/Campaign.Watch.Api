using Campaign.Watch.Domain.Entities.Alerts;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Repositories.Alerts
{
    public interface IAlertHistoryRepository
    {
        Task<IEnumerable<AlertHistoryEntity>> GetByScopeAsync(ObjectId? clientId, CancellationToken cancellationToken, int limit = 100);
        Task<AlertHistoryEntity> CreateAsync(AlertHistoryEntity entity);
    }
}
