using Campaign.Watch.Domain.Entities.Read.Campaign;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Read.Campaign
{
    public interface ICampaignMonitorApplication
    {
        #region Banco de origem (monitoramento)

        Task<IEnumerable<CampaignRead>> GetSourceCampaignsByClientAsync(string dbName);
        Task<IEnumerable<ExecutionRead>> GetSourceExecutionsByCampaignAsync(string dbName, string campaignId);
        Task<CampaignRead> GetSourceCampaignByIdAsync(string dbName, string campaignId);

        #endregion
    }
}
