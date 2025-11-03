using Campaign.Watch.Application.Dtos.Alerts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Alerts
{
    public interface IAlertApplication
    {
        Task<IEnumerable<AlertConfigurationResponse>> GetAllAlertsAsync(string clientId);
        Task<AlertConfigurationResponse> GetAlertByIdAsync(string id);
        Task<AlertConfigurationResponse> CreateAlertAsync(SaveAlertConfigurationRequest request);
        Task<bool> UpdateAlertAsync(string id, SaveAlertConfigurationRequest request);
        Task<bool> DeleteAlertAsync(string id);
        Task<IEnumerable<AlertHistoryResponse>> GetAlertHistoryAsync(string clientId);
    }
}
