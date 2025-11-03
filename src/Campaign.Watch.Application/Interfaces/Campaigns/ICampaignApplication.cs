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
        Task<PaginatedResponse<CampaignMonitoringResponse>> ObterCampanhasMonitoradasAsync(string clientName, string monitoringStatus, bool? hasErrors, DateTime? dataInicio,
            DateTime? dataFim, int pagina, int tamanhoPagina);
        Task<CampaignMetricsDto> ObterMetricasCampanhaAsync(string campaignId);
    }
}
