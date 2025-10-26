using Campaign.Watch.Application.Dtos.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Campaign
{
    public interface IDashboardApplication
    {
        Task<MonitoringDashboardResponse> ObterDadosDashboardAsync(string clientName = null);
        Task<IEnumerable<UpcomingExecutionDto>> ObterProximasExecucoesAsync(int proximasHoras = 24);
        Task<IEnumerable<CampaignStatusGroupDto>> ObterContagemPorStatusMonitoramentoAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<IEnumerable<CampaignHealthGroupDto>> ObterContagemPorNivelSaudeAsync(string clientName = null);
        Task<Dictionary<string, double>> ObterTaxaSucessoExecucoesAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
