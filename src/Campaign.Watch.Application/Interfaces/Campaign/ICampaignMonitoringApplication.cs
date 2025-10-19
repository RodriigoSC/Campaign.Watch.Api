using Campaign.Watch.Application.Dtos.Campaign;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Campaign
{
    public interface ICampaignMonitoringApplication
    {
        Task<CampaignMonitoringResponse> ObterCampanhaMonitoradaPorIdAsync(string id);
        Task<CampaignMonitoringResponse> ObterCampanhaMonitoradaPorIdCampanhaAsync(string clientName, string idCampanha);
        Task<IEnumerable<CampaignMonitoringResponse>> ObterCampanhasMonitoradasPorClienteAsync(string clientName);
        Task<IEnumerable<CampaignMonitoringResponse>> ObterCampanhasMonitoradasAsync(string clientName = null, string monitoringStatus = null, bool? hasErrors = null, DateTime? dataInicio = null, DateTime? dataFim = null, int pagina = 1, int tamanhoPagina = 50);

        Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesPorCampanhaAsync(string campaignMonitoringId);
        Task<ExecutionMonitoringResponse> ObterExecucaoPorIdAsync(string executionId);
        Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesComErrosAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<CampaignDiagnosticResponse> ObterDiagnosticoCampanhaAsync(string campaignId);
        Task<IEnumerable<DiagnosticIssueDto>> ObterProblemasDetectadosAsync(string severity = null, DateTime? desde = null, int limite = 100);
        Task<CampaignMetricsDto> ObterMetricasCampanhaAsync(string campaignId);
        Task<MonitoringDashboardResponse> ObterDadosDashboardAsync(string clientName = null);
        Task<IEnumerable<UpcomingExecutionDto>> ObterProximasExecucoesAsync(int proximasHoras = 24);
        Task<IEnumerable<CampaignStatusGroupDto>> ObterContagemPorStatusMonitoramentoAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<CampaignHealthGroupDto>> ObterContagemPorNivelSaudeAsync(string clientName = null);
        Task<Dictionary<string, double>> ObterTaxaSucessoExecucoesAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
