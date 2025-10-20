using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Interfaces.Campaign;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutionMonitoringController : ControllerBase
    {

        private readonly ICampaignMonitoringApplication _monitoringApp;
        private readonly ILogger<ExecutionMonitoringController> _logger;

        public ExecutionMonitoringController(ICampaignMonitoringApplication monitoringApp, ILogger<ExecutionMonitoringController> logger)
        {
            _monitoringApp = monitoringApp ?? throw new ArgumentNullException(nameof(monitoringApp));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém os detalhes de uma execução específica pelo seu ID original.
        /// </summary>
        /// <param name="id">O ID original da execução (gerado pela plataforma de origem).</param>
        /// <returns>Os detalhes da execução monitorada.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExecutionMonitoringResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObterExecucaoPorId(string id)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/executions/{Id}", id);
            if (string.IsNullOrWhiteSpace(id)) // Validação básica do ID
            {
                return BadRequest("ID da execução inválido.");
            }

            var execution = await _monitoringApp.ObterExecucaoPorIdAsync(id);
            if (execution == null)
            {
                return NotFound($"Execução com ID '{id}' não encontrada.");
            }
            return Ok(execution);
        }

        /// <summary>
        /// Obtém uma lista de execuções que apresentaram erros de monitoramento, com filtros opcionais.
        /// </summary>
        /// <param name="clientName">Filtra por nome do cliente.</param>
        /// <param name="dataInicio">Data de início (baseada no início da execução) para o filtro.</param>
        /// <param name="dataFim">Data de fim (baseada no início da execução) para o filtro.</param>
        /// <returns>Uma lista de execuções com erros.</returns>
        [HttpGet("with-errors")]
        [ProducesResponseType(typeof(IEnumerable<ExecutionMonitoringResponse>), 200)]
        public async Task<IActionResult> ObterExecucoesComErros(
            [FromQuery] string clientName = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            _logger.LogInformation("Recebida requisição GET /api/monitoring/executions/with-errors");
            var executions = await _monitoringApp.ObterExecucoesComErrosAsync(clientName, dataInicio, dataFim);
            return Ok(executions);
        }
    }
}
