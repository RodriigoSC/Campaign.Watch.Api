using Campaign.Watch.Application.Dtos;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Worker
{
    public interface ICampaignMonitorFlow
    {
        Task MonitorarCampanhasAsync();
    }
}
