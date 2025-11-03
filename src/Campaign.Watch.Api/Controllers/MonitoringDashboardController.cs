using Campaign.Watch.Application.Dtos.Dashboard;
using Campaign.Watch.Application.Dtos.Diagnostic;
using Campaign.Watch.Application.Interfaces.Campaign;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringDashboardController : ControllerBase
    {
        // Serviços de aplicação divididos
        private readonly IDashboardApplication _dashboardApp;
        private readonly IDiagnosticApplication _diagnosticApp;
        private readonly ILogger<MonitoringDashboardController> _logger;

        public MonitoringDashboardController(
            IDashboardApplication dashboardApp,
            IDiagnosticApplication diagnosticApp,
            ILogger<MonitoringDashboardController> logger)
        {
            _dashboardApp = dashboardApp ?? throw new ArgumentNullException(nameof(dashboardApp));
            _diagnosticApp = diagnosticApp ?? throw new ArgumentNullException(nameof(diagnosticApp));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém os dados consolidados para o dashboard principal.
        /// </summary>
        /// <param name="clientName">Filtra os dados do dashboard para um cliente específico.</param>
        /// <param name="dataInicio">Data de início para filtrar as campanhas (baseado no CreatedAt).</param>
        /// <param name="dataFim">Data de fim para filtrar as campanhas (baseado no CreatedAt).</param>
        /// <returns>Um objeto contendo o resumo, agrupamentos e listas para o dashboard.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(MonitoringDashboardResponse), 200)]
        public async Task<IActionResult> ObterDadosDashboard(
            [FromQuery] string clientName = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/dashboard");

            var dashboardData = await _dashboardApp.ObterDadosDashboardAsync(clientName, dataInicio, dataFim);

            return Ok(dashboardData);
        }

        /// <summary>
        /// Obtém a lista das próximas execuções de campanhas agendadas (padrão: próximas 24 horas).
        /// </summary>
        /// <param name="proximasHoras">Define o período em horas para buscar as próximas execuções (padrão 24).</param>
        /// <returns>Uma lista das próximas execuções agendadas.</returns>
        [HttpGet("upcoming-executions")]
        [ProducesResponseType(typeof(IEnumerable<UpcomingExecutionDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterProximasExecucoes([FromQuery] int proximasHoras = 24)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/dashboard/upcoming-executions");
            if (proximasHoras <= 0)
            {
                return BadRequest("O parâmetro 'proximasHoras' deve ser positivo.");
            }
            var upcoming = await _dashboardApp.ObterProximasExecucoesAsync(proximasHoras);
            return Ok(upcoming);
        }

        /// <summary>
        /// Obtém uma lista dos problemas mais recentes detectados pelo monitoramento.
        /// </summary>
        /// <param name="severity">Filtra problemas por severidade (ex: Error, Warning).</param>
        /// <param name="desde">Filtra problemas detectados a partir desta data/hora.</param>
        /// <param name="limite">Número máximo de problemas a retornar (padrão 100).</param>
        /// <returns>Uma lista dos problemas recentes.</returns>
        [HttpGet("recent-issues")]
        [ProducesResponseType(typeof(IEnumerable<DiagnosticIssueDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterProblemasRecentes(
            [FromQuery] string severity = null,
            [FromQuery] DateTime? desde = null,
            [FromQuery] int limite = 100)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/dashboard/recent-issues");
            if (limite <= 0 || limite > 1000) // Limitar busca
            {
                return BadRequest("O parâmetro 'limite' deve estar entre 1 e 1000.");
            }
            // Lógica movida para o serviço de Diagnóstico
            var issues = await _diagnosticApp.ObterProblemasDetectadosAsync(severity, desde, limite);
            return Ok(issues);
        }

        /// <summary>
        /// Obtém a contagem de campanhas agrupadas por status de monitoramento.
        /// </summary>
        /// <param name="clientName">Filtra por nome do cliente.</param>
        /// <param name="dataInicio">Data de início (criação da campanha) para filtro.</param>
        /// <param name="dataFim">Data de fim (criação da campanha) para filtro.</param>
        /// <returns>Contagem e percentual de campanhas por status de monitoramento.</returns>
        [HttpGet("stats/by-monitoring-status")]
        [ProducesResponseType(typeof(IEnumerable<CampaignStatusGroupDto>), 200)]
        public async Task<IActionResult> ObterContagemPorStatusMonitoramento(
            [FromQuery] string clientName = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/dashboard/stats/by-monitoring-status");
            var counts = await _dashboardApp.ObterContagemPorStatusMonitoramentoAsync(clientName, dataInicio, dataFim);
            return Ok(counts);
        }

        /// <summary>
        /// Obtém a contagem de campanhas agrupadas por nível de saúde (ex: Healthy, Warning, Error).
        /// </summary>
        /// <param name="clientName">Filtra por nome do cliente.</param>
        /// <returns>Contagem de campanhas por nível de saúde.</returns>
        [HttpGet("stats/by-health-level")]
        [ProducesResponseType(typeof(IEnumerable<CampaignHealthGroupDto>), 200)]
        public async Task<IActionResult> ObterContagemPorNivelSaude([FromQuery] string clientName = null)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/dashboard/stats/by-health-level");
            var groups = await _dashboardApp.ObterContagemPorNivelSaudeAsync(clientName);
            return Ok(groups);
        }

        /// <summary>
        /// Obtém a taxa de sucesso agregada das execuções de campanhas.
        /// </summary>
        /// <param name="clientName">Filtra por nome do cliente.</param>
        /// <param name="dataInicio">Data de início (início da execução) para filtro.</param>
        /// <param name="dataFim">Data de fim (início da execução) para filtro.</param>
        /// <returns>Um dicionário com total de execuções, sucessos, erros e taxas percentuais.</returns>
        [HttpGet("stats/success-rate")]
        [ProducesResponseType(typeof(Dictionary<string, double>), 200)]
        public async Task<IActionResult> ObterTaxaSucessoExecucoes(
            [FromQuery] string clientName = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/dashboard/stats/success-rate");
            var rates = await _dashboardApp.ObterTaxaSucessoExecucoesAsync(clientName, dataInicio, dataFim);
            return Ok(rates);
        }
    }
}