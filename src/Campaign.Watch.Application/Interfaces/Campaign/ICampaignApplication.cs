using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Campaign
{
    public interface ICampaignApplication
    {
        #region Banco de persistência

        Task<CampaignDetailResponse> CriarCampanhaAsync(CampaignDetailResponse dto);
        Task<bool> AtualizarCampanhaAsync(string id, CampaignDetailResponse dto);
        Task<IEnumerable<CampaignDetailResponse>> ObterTodasAsCampanhasAsync();
        Task<CampaignDetailResponse> ObterCampanhaPorIdAsync(string id);
        Task<CampaignDetailResponse> ObterCampanhaPorNomeAsync(string nomeCampanha);
        Task<CampaignDetailResponse> ObterCampanhaPorNumeroAsync(long numeroCampanha);
        Task<IEnumerable<CampaignDetailResponse>> ObterTodasAsCampanhasPorClienteAsync(string nomeCliente);
        Task<IEnumerable<CampaignDetailResponse>> ObterCampanhasPaginadasAsync(int pagina, int tamanhoPagina);
        Task<CampaignDetailResponse> ObterCampanhaPorIdCampanhaAsync(string nomeCliente, string idCampanha);
        Task<IEnumerable<CampaignDetailResponse>> ObterCampanhasParaMonitorarAsync();
        Task<IEnumerable<CampaignErrorResponse>> ObterCampanhasComErrosDeIntegracaoAsync();
        Task<IEnumerable<CampaignDelayedResponse>> ObterCampanhasComExecucaoAtrasadaAsync();
        Task<IEnumerable<CampaignStatusResponse>> ObterCampanhasMonitoradasComSucessoAsync();

        Task<IEnumerable<CampaignStatusCountResponse>> ObterCampanhasPorStatusAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim);
        Task<IEnumerable<CampaignStatusCountResponse>> ObterCampanhasPorStatusMonitoramentoAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim);
        #endregion
    }
}