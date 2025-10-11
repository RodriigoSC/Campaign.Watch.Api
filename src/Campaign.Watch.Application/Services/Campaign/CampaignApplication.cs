using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Campaign
{
    public class CampaignApplication : ICampaignApplication
    {
        private readonly ICampaignService _campaignService;
        private readonly IMapper _mapper;

        public CampaignApplication(ICampaignService campaignService, IMapper mapper)
        {
            _campaignService = campaignService;
            _mapper = mapper;
        }

        #region Banco de persistência

        public async Task<CampaignDetailResponse> CriarCampanhaAsync(CampaignDetailResponse dto)
        {
            var entity = _mapper.Map<CampaignEntity>(dto);
            var created = await _campaignService.CriarCampanhaAsync(entity);
            return _mapper.Map<CampaignDetailResponse>(created);
        }

        public async Task<bool> AtualizarCampanhaAsync(string id, CampaignDetailResponse dto)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            var entity = _mapper.Map<CampaignEntity>(dto);
            return await _campaignService.AtualizarCampanhaAsync(objectId, entity);
        }

        public async Task<IEnumerable<CampaignDetailResponse>> ObterTodasAsCampanhasAsync()
        {
            var campaigns = await _campaignService.ObterTodasAsCampanhasAsync();
            return _mapper.Map<IEnumerable<CampaignDetailResponse>>(campaigns);
        }

        public async Task<CampaignDetailResponse> ObterCampanhaPorIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null;
            }
            var campaign = await _campaignService.ObterCampanhaPorIdAsync(objectId);
            return _mapper.Map<CampaignDetailResponse>(campaign);
        }

        public async Task<CampaignDetailResponse> ObterCampanhaPorNomeAsync(string nomeCampanha)
        {
            var campaign = await _campaignService.ObterCampanhaPorNomeAsync(nomeCampanha);
            return _mapper.Map<CampaignDetailResponse>(campaign);
        }

        public async Task<CampaignDetailResponse> ObterCampanhaPorNumeroAsync(long numeroCampanha)
        {
            var campaign = await _campaignService.ObterCampanhaPorNumeroAsync(numeroCampanha);
            return _mapper.Map<CampaignDetailResponse>(campaign);
        }

        public async Task<IEnumerable<CampaignDetailResponse>> ObterTodasAsCampanhasPorClienteAsync(string nomeCliente)
        {
            var campaigns = await _campaignService.ObterTodasAsCampanhasPorClienteAsync(nomeCliente);
            return _mapper.Map<IEnumerable<CampaignDetailResponse>>(campaigns);
        }

        public async Task<IEnumerable<CampaignDetailResponse>> ObterCampanhasPaginadasAsync(int pagina, int tamanhoPagina)
        {
            var campaigns = await _campaignService.ObterCampanhasPaginadasAsync(pagina, tamanhoPagina);
            return _mapper.Map<IEnumerable<CampaignDetailResponse>>(campaigns);
        }

        public async Task<CampaignDetailResponse> ObterCampanhaPorIdCampanhaAsync(string nomeCliente, string idCampanha)
        {
            var campaign = await _campaignService.ObterCampanhaPorIdCampanhaAsync(nomeCliente, idCampanha);
            return _mapper.Map<CampaignDetailResponse>(campaign);
        }

        public async Task<IEnumerable<CampaignDetailResponse>> ObterCampanhasParaMonitorarAsync()
        {
            var campaigns = await _campaignService.ObterCampanhasParaMonitorarAsync();
            return _mapper.Map<IEnumerable<CampaignDetailResponse>>(campaigns);
        }

        public async Task<IEnumerable<CampaignErrorResponse>> ObterCampanhasComErrosDeIntegracaoAsync()
        {
            var campaigns = await _campaignService.ObterCampanhasComErrosDeIntegracaoAsync();
            return _mapper.Map<IEnumerable<CampaignErrorResponse>>(campaigns);
        }

        public async Task<IEnumerable<CampaignDelayedResponse>> ObterCampanhasComExecucaoAtrasadaAsync()
        {
            var campaigns = await _campaignService.ObterCampanhasComExecucaoAtrasadaAsync();
            return _mapper.Map<IEnumerable<CampaignDelayedResponse>>(campaigns);
        }

        public async Task<IEnumerable<CampaignStatusResponse>> ObterCampanhasMonitoradasComSucessoAsync()
        {
            var campaigns = await _campaignService.ObterCampanhasMonitoradasComSucessoAsync();
            return _mapper.Map<IEnumerable<CampaignStatusResponse>>(campaigns);
        }

        public async Task<IEnumerable<CampaignStatusCountResponse>> ObterCampanhasPorStatusAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim)
        {
            var counts = await _campaignService.ObterCampanhasPorStatusAsync(nomeCliente, dataInicio, dataFim);
            return _mapper.Map<IEnumerable<CampaignStatusCountResponse>>(counts);
        }

        public async Task<IEnumerable<CampaignStatusCountResponse>> ObterCampanhasPorStatusMonitoramentoAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim)
        {
            var campaigns = await _campaignService.ObterCampanhasPorStatusMonitoramentoAsync(nomeCliente, dataInicio, dataFim);
            return _mapper.Map<IEnumerable<CampaignStatusCountResponse>>(campaigns);
        }

        #endregion
    }
}