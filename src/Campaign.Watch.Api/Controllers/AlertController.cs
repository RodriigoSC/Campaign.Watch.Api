using Campaign.Watch.Application.Dtos.Alerts;
using Campaign.Watch.Application.Interfaces.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertApplication _alertApp;
        private readonly ILogger<AlertController> _logger;

        public AlertController(IAlertApplication alertApp, ILogger<AlertController> logger)
        {
            _alertApp = alertApp;
            _logger = logger;
        }

        #region Alert Configuration

        /// <summary>
        /// Busca configurações de alerta por escopo (global ou de cliente).
        /// </summary>
        /// <param name="clientId">Para alertas de cliente, passe o ID. Para globais, passe 'global'.</param>
        /// <returns>Lista de configurações de alerta.</returns>
        [HttpGet("AlertConfiguration")]
        [ProducesResponseType(typeof(IEnumerable<AlertConfigurationResponse>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllAlerts([FromQuery] string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest("O parâmetro 'clientId' (com ID do cliente ou 'global') é obrigatório.");
            }

            try
            {
                var alerts = await _alertApp.GetAllAlertsAsync(clientId);
                return Ok(alerts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Busca uma configuração de alerta específica por ID.
        /// </summary>
        [HttpGet("AlertConfiguration/{id}", Name = "GetAlertById")]
        [ProducesResponseType(typeof(AlertConfigurationResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAlertById(string id)
        {
            var alert = await _alertApp.GetAlertByIdAsync(id);
            if (alert == null)
            {
                return NotFound();
            }
            return Ok(alert);
        }

        /// <summary>
        /// Cria uma nova configuração de alerta (global ou de cliente).
        /// </summary>
        /// <param name="request">Dados do alerta. Para global, ClientId deve ser nulo.</param>
        [HttpPost("AlertConfiguration")]
        [ProducesResponseType(typeof(AlertConfigurationResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAlert([FromBody] SaveAlertConfigurationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdAlert = await _alertApp.CreateAlertAsync(request);
                return CreatedAtAction(nameof(GetAlertById), new { id = createdAlert.Id }, createdAlert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar alerta.");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma configuração de alerta existente.
        /// </summary>
        [HttpPut("AlertConfiguration/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAlert(string id, [FromBody] SaveAlertConfigurationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _alertApp.UpdateAlertAsync(id, request);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar alerta {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Exclui uma configuração de alerta.
        /// </summary>
        [HttpDelete("AlertConfiguration/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAlert(string id)
        {
            var success = await _alertApp.DeleteAlertAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        #endregion

        #region Alert History

        /// <summary>
        /// Busca o histórico de alertas disparados por escopo (global ou de cliente).
        /// </summary>
        /// <param name="clientId">Para histórico de cliente, passe o ID. Para globais, passe 'global'.</param>
        /// <returns>Lista de alertas históricos.</returns>
        [HttpGet("AlertHistory")]
        [ProducesResponseType(typeof(IEnumerable<AlertHistoryResponse>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAlertHistory([FromQuery] string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest("O parâmetro 'clientId' (com ID do cliente ou 'global') é obrigatório.");
            }

            try
            {
                var history = await _alertApp.GetAlertHistoryAsync(clientId);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
