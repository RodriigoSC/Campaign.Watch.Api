using Campaign.Watch.Application.Dtos.Campaign;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Campaign
{
    public interface ICampaignApplication
    {
        Task<CampaignMonitoringResponse> ObterCampanhaMonitoradaPorIdAsync(string id);
        Task<CampaignMonitoringResponse> ObterCampanhaMonitoradaPorIdCampanhaAsync(string clientName, string idCampanha);
        Task<IEnumerable<CampaignMonitoringResponse>> ObterCampanhasMonitoradasPorClienteAsync(string clientName);
        Task<IEnumerable<CampaignMonitoringResponse>> ObterCampanhasMonitoradasAsync(string clientName = null, string monitoringStatus = null, bool? hasErrors = null, DateTime? dataInicio = null, DateTime? dataFim = null, int pagina = 1, int tamanhoPagina = 50);
        Task<CampaignMetricsDto> ObterMetricasCampanhaAsync(string campaignId);
    }
}
