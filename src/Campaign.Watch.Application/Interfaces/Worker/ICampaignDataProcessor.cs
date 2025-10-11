using Campaign.Watch.Application.Dtos.Client;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Entities.Read.Campaign;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Worker
{
    public interface ICampaignDataProcessor
    {
        Task<CampaignEntity> ProcessAndEnrichCampaignDataAsync(ClientResponse client, CampaignRead campaignSource);
    }
}
