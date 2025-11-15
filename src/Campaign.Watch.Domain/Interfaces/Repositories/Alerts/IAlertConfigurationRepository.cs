using Campaign.Watch.Domain.Entities.Alerts;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Repositories.Alerts
{
    public interface IAlertConfigurationRepository
    {
        Task<IEnumerable<AlertConfigurationEntity>> GetByScopeAsync(ObjectId? clientId, CancellationToken cancellationToken);
        Task<AlertConfigurationEntity> GetByIdAsync(ObjectId id);
        Task<AlertConfigurationEntity> CreateAsync(AlertConfigurationEntity entity);
        Task<bool> UpdateAsync(ObjectId id, AlertConfigurationEntity entity);
        Task<bool> DeleteAsync(ObjectId id);
    }
}
