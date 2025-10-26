using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Dtos.Diagnostic;
using Campaign.Watch.Application.Dtos.Execution;
using Campaign.Watch.Application.Interfaces.Campaign;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignMonitoringController : ControllerBase
    {
        private readonly ICampaignApplication _monitoringApp;
        private readonly IExecutionApplication _executionApp;
        private readonly IDiagnosticApplication _diagnosticApp;
        private readonly ILogger<CampaignMonitoringController> _logger;

        public CampaignMonitoringController(
            ICampaignApplication monitoringApp,
            IExecutionApplication executionApp,
            IDiagnosticApplication diagnosticApp,
            ILogger<CampaignMonitoringController> logger)
        {
            _monitoringApp = monitoringApp ?? throw new ArgumentNullException(nameof(monitoringApp));
            _executionApp = executionApp ?? throw new ArgumentNullException(nameof(executionApp));
            _diagnosticApp = diagnosticApp ?? throw new ArgumentNullException(nameof(diagnosticApp));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém uma lista paginada de campanhas monitoradas, com opções de filtro.
        /// </summary>
        /// <param name="clientName">Filtra por nome do cliente.</param>
        /// <param name="monitoringStatus">Filtra por status de monitoramento (ex: Pending, InProgress, Completed, Failed, ExecutionDelayed, WaitingForNextExecution).</param>
        /// <param name="hasErrors">Filtra campanhas com (true) ou sem (false) erros de integração.</param>
        /// <param name="dataInicio">Data de início (baseada na criação da campanha) para o filtro.</param>
        /// <param name="dataFim">Data de fim (baseada na criação da campanha) para o filtro.</param>
        /// <param name="pagina">Número da página (padrão 1).</param>
        /// <param name="tamanhoPagina">Número de itens por página (padrão 50).</param>
        /// <returns>Uma lista de campanhas monitoradas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CampaignMonitoringResponse>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterCampanhasMonitoradas(
            [FromQuery] string clientName = null,
            [FromQuery] string monitoringStatus = null,
            [FromQuery] bool? hasErrors = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 50)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns");
            // Validação básica de paginação
            if (pagina < 1 || tamanhoPagina < 1 || tamanhoPagina > 1000) // Limite máximo de 1000 por página
            {
                return BadRequest("Parâmetros de paginação inválidos.");
            }
            var campaigns = await _monitoringApp.ObterCampanhasMonitoradasAsync(clientName, monitoringStatus, hasErrors, dataInicio, dataFim, pagina, tamanhoPagina);
            return Ok(campaigns);
        }

        /// <summary>
        /// Obtém os detalhes de uma campanha monitorada específica pelo seu ID de monitoramento (ObjectId).
        /// </summary>
        /// <param name="id">O ID de monitoramento da campanha (ObjectId string).</param>
        /// <returns>Os detalhes da campanha monitorada.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CampaignMonitoringResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterCampanhaMonitoradaPorId(string id)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns/{Id}", id);
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("ID da campanha inválido.");
            }
            var campaign = await _monitoringApp.ObterCampanhaMonitoradaPorIdAsync(id);
            if (campaign == null)
            {
                return NotFound($"Campanha monitorada com ID '{id}' não encontrada.");
            }
            return Ok(campaign);
        }

        /// <summary>
        /// Obtém as campanhas monitoradas para um cliente específico.
        /// </summary>
        /// <param name="clientName">O nome do cliente.</param>
        /// <returns>Lista de campanhas monitoradas para o cliente.</returns>
        [HttpGet("client/{clientName}")]
        [ProducesResponseType(typeof(IEnumerable<CampaignMonitoringResponse>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterCampanhasPorCliente(string clientName)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns/client/{ClientName}", clientName);
            if (string.IsNullOrWhiteSpace(clientName))
            {
                return BadRequest("Nome do cliente é obrigatório.");
            }
            var campaigns = await _monitoringApp.ObterCampanhasMonitoradasPorClienteAsync(clientName);
            return Ok(campaigns);
        }


        /// <summary>
        /// Obtém os detalhes de uma campanha monitorada específica pelo nome do cliente e ID original da campanha.
        /// </summary>
        /// <param name="clientName">O nome do cliente.</param>
        /// <param name="idCampanha">O ID original da campanha na plataforma de origem.</param>
        /// <returns>Os detalhes da campanha monitorada.</returns>
        [HttpGet("original/{clientName}/{idCampanha}")]
        [ProducesResponseType(typeof(CampaignMonitoringResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterCampanhaMonitoradaPorIdOriginal(string clientName, string idCampanha)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns/original/{ClientName}/{IdCampanha}", clientName, idCampanha);
            if (string.IsNullOrWhiteSpace(clientName) || string.IsNullOrWhiteSpace(idCampanha))
            {
                return BadRequest("Nome do cliente e ID original da campanha são obrigatórios.");
            }
            var campaign = await _monitoringApp.ObterCampanhaMonitoradaPorIdCampanhaAsync(clientName, idCampanha);
            if (campaign == null)
            {
                return NotFound($"Campanha monitorada para o cliente '{clientName}' com ID original '{idCampanha}' não encontrada.");
            }
            return Ok(campaign);
        }

        /// <summary>
        /// Obtém as métricas calculadas para uma campanha monitorada específica.
        /// </summary>
        /// <param name="id">O ID de monitoramento da campanha (ObjectId string).</param>
        /// <returns>As métricas da campanha.</returns>
        [HttpGet("{id}/metrics")]
        [ProducesResponseType(typeof(CampaignMetricsDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterMetricasCampanha(string id)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns/{Id}/metrics", id);
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("ID da campanha inválido.");
            }
            var metrics = await _monitoringApp.ObterMetricasCampanhaAsync(id);
            if (metrics == null)
            {
                return NotFound($"Métricas não encontradas ou campanha inexistente para o ID '{id}'.");
            }
            return Ok(metrics);
        }

        /// <summary>
        /// Obtém o diagnóstico completo (problemas e recomendações) para uma campanha monitorada específica.
        /// </summary>
        /// <param name="id">O ID de monitoramento da campanha (ObjectId string).</param>
        /// <returns>O diagnóstico da campanha.</returns>
        [HttpGet("{id}/diagnostic")]
        [ProducesResponseType(typeof(CampaignDiagnosticResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterDiagnosticoCampanha(string id)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns/{Id}/diagnostic", id);
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("ID da campanha inválido.");
            }
            // Alterado para _diagnosticApp
            var diagnostic = await _diagnosticApp.ObterDiagnosticoCampanhaAsync(id);
            if (diagnostic == null)
            {
                return NotFound($"Diagnóstico não encontrado ou campanha inexistente para o ID '{id}'.");
            }
            return Ok(diagnostic);
        }

        /// <summary>
        /// Obtém todas as execuções associadas a uma campanha monitorada específica.
        /// </summary>
        /// <param name="id">O ID de monitoramento da campanha (ObjectId string).</param>
        /// <returns>Uma lista das execuções da campanha.</returns>
        [HttpGet("{id}/executions")]
        [ProducesResponseType(typeof(IEnumerable<ExecutionMonitoringResponse>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterExecucoesDaCampanha(string id)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/campaigns/{Id}/executions", id);
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("ID da campanha inválido.");
            }
            // Alterado para _executionApp
            var executions = await _executionApp.ObterExecucoesPorCampanhaAsync(id);

            return Ok(executions);
        }
    }
}