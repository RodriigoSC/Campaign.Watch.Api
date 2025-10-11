using AutoMapper;
using Campaign.Watch.Application.Interfaces.Read.Campaign;
using Campaign.Watch.Domain.Entities.Read.Campaign;
using Campaign.Watch.Domain.Interfaces.Services.Read.Campaign;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Read.Campaign
{
    public class CampaignMonitorApplication : ICampaignMonitorApplication
    {
        private readonly ICampaignReadService _campaignReadService;
        private readonly IMapper _mapper;

        public CampaignMonitorApplication(ICampaignReadService campaignReadService, IMapper mapper)
        {
            _campaignReadService = campaignReadService;
            _mapper = mapper;
        }


        #region Banco de origem Campaign (monitoramento)

        public async Task<IEnumerable<CampaignRead>> GetSourceCampaignsByClientAsync(string dbName)
        {
            var sourceCampaigns = await _campaignReadService.GetCampaignsByClient(dbName);
            return _mapper.Map<IEnumerable<CampaignRead>>(sourceCampaigns);
        }

        public async Task<IEnumerable<ExecutionRead>> GetSourceExecutionsByCampaignAsync(string dbName, string campaignId)
        {
            var sourceExecutions = await _campaignReadService.GetExecutionsByCampaign(dbName, campaignId);
            return _mapper.Map<IEnumerable<ExecutionRead>>(sourceExecutions);
        }

        public async Task<CampaignRead> GetSourceCampaignByIdAsync(string dbName, string campaignId)
        {
            var sourceCampaign = await _campaignReadService.GetCampaignById(dbName, campaignId);
            return _mapper.Map<CampaignRead>(sourceCampaign);
        }

        #endregion

    }
}
