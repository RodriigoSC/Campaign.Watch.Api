using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    /// <summary>
    /// Controller da API para consultar informações sobre as campanhas monitoradas.
    /// </summary>
    [Route("api/campanhas")] // Rota principal também traduzida
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignApplication _campaignApplication;

        public CampaignController(ICampaignApplication campaignApplication)
        {
            _campaignApplication = campaignApplication;
        }

        /// <summary>
        /// Obtém uma campanha específica pelo seu ID único.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CampaignDetailResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorId(string id)
        {
            // CORREÇÃO: Chamando o método traduzido da camada de aplicação
            var campaign = await _campaignApplication.ObterCampanhaPorIdAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }
            return Ok(campaign);
        }

        /// <summary>
        /// Obtém uma campanha específica pelo seu nome.
        /// </summary>
        [HttpGet("por-nome/{nomeCampanha}")]
        [ProducesResponseType(typeof(CampaignDetailResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorNome(string nomeCampanha)
        {
            var campaign = await _campaignApplication.ObterCampanhaPorNomeAsync(nomeCampanha);
            if (campaign == null)
            {
                return NotFound();
            }
            return Ok(campaign);
        }

        /// <summary>
        /// Obtém uma campanha específica pelo seu ID numérico.
        /// </summary>
        [HttpGet("por-numero/{numeroCampanha}")]
        [ProducesResponseType(typeof(CampaignDetailResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorNumero(long numeroCampanha)
        {
            var campaign = await _campaignApplication.ObterCampanhaPorNumeroAsync(numeroCampanha);
            if (campaign == null)
            {
                return NotFound();
            }
            return Ok(campaign);
        }

        /// <summary>
        /// Obtém todas as campanhas de um cliente específico.
        /// </summary>
        [HttpGet("por-cliente/{nomeCliente}")]
        [ProducesResponseType(typeof(IEnumerable<CampaignDetailResponse>), 200)]
        public async Task<IActionResult> ObterPorCliente(string nomeCliente)
        {
            var campaigns = await _campaignApplication.ObterTodasAsCampanhasPorClienteAsync(nomeCliente);
            return Ok(campaigns);
        }

        /// <summary>
        /// Obtém uma lista paginada de campanhas.
        /// </summary>
        [HttpGet("paginado")]
        [ProducesResponseType(typeof(IEnumerable<CampaignDetailResponse>), 200)]
        public async Task<IActionResult> ObterPaginado([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
        {
            var campaigns = await _campaignApplication.ObterCampanhasPaginadasAsync(pagina, tamanhoPagina);
            return Ok(campaigns);
        }

        /// <summary>
        /// Obtém todas as campanhas com erro de integração.
        /// </summary>
        [HttpGet("com-erros-integracao")]
        [ProducesResponseType(typeof(IEnumerable<CampaignErrorResponse>), 200)]
        public async Task<IActionResult> ObterComErrosDeIntegracao()
        {
            var campaigns = await _campaignApplication.ObterCampanhasComErrosDeIntegracaoAsync();
            return Ok(campaigns);
        }

        /// <summary>
        /// Obtém todas as campanhas com execução atrasada.
        /// </summary>
        [HttpGet("com-execucao-atrasada")]
        [ProducesResponseType(typeof(IEnumerable<CampaignDelayedResponse>), 200)]
        public async Task<IActionResult> ObterComExecucaoAtrasada()
        {
            var campaigns = await _campaignApplication.ObterCampanhasComExecucaoAtrasadaAsync();
            return Ok(campaigns);
        }

        /// <summary>
        /// Obtém um resumo das campanhas monitoradas com sucesso.
        /// </summary>
        [HttpGet("monitoradas-com-sucesso")]
        [ProducesResponseType(typeof(IEnumerable<CampaignStatusResponse>), 200)]
        public async Task<IActionResult> ObterMonitoradasComSucesso()
        {
            var campaigns = await _campaignApplication.ObterCampanhasMonitoradasComSucessoAsync();
            return Ok(campaigns);
        }

        /// <summary>
        /// Obtém a contagem de campanhas agrupadas por status de monitoramento.
        /// </summary>
        [HttpGet("contagem-por-status-monitoramento")]
        [ProducesResponseType(typeof(IEnumerable<CampaignStatusCountResponse>), 200)]
        public async Task<IActionResult> ObterContagemPorStatusMonitoramento(
            [FromQuery] string nomeCliente,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            var counts = await _campaignApplication.ObterCampanhasPorStatusMonitoramentoAsync(nomeCliente, dataInicio, dataFim);
            return Ok(counts);
        }

        /// <summary>
        /// Obtém a contagem de campanhas agrupadas por status da campanha (ex: Executando, Concluída).
        /// </summary>
        [HttpGet("contagem-por-status-campanha")]
        [ProducesResponseType(typeof(IEnumerable<CampaignStatusCountResponse>), 200)]
        public async Task<IActionResult> ObterContagemPorStatusCampanha(
            [FromQuery] string nomeCliente,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            var counts = await _campaignApplication.ObterCampanhasPorStatusAsync(nomeCliente, dataInicio, dataFim);
            return Ok(counts);
        }


    }
}