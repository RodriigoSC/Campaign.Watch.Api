using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Campaign
{
    public interface ICampaignService
    {
        Task<CampaignEntity> CriarCampanhaAsync(CampaignEntity entity);
        Task<bool> AtualizarCampanhaAsync(ObjectId id, CampaignEntity entity);
        Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasAsync();
        Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorClienteAsync(string nomeCliente);
        Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorDataAsync(DateTime inicio, DateTime fim);
        Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorClienteOuDataAsync(string nomeCliente, DateTime inicio, DateTime fim);
        Task<CampaignEntity> ObterCampanhaPorIdAsync(ObjectId id);
        Task<CampaignEntity> ObterCampanhaPorNomeAsync(string nomeCampanha);
        Task<CampaignEntity> ObterCampanhaPorNumeroAsync(long numeroCampanha);
        Task<CampaignEntity> ObterCampanhaPorIdCampanhaAsync(string nomeCliente, string idCampanha);
        Task<IEnumerable<CampaignEntity>> ObterCampanhasAtivasAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasPaginadasAsync(int pagina, int tamanhoPagina);
        Task<int> ContarCampanhasPorClienteAsync(string nomeCliente);
        Task<IEnumerable<CampaignEntity>> ObterCampanhasParaMonitorarAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasComErrosDeIntegracaoAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasComExecucaoAtrasadaAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasMonitoradasComSucessoAsync();

        Task<IEnumerable<CampaignStatusCount>> ObterCampanhasPorStatusAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim);
        Task<IEnumerable<CampaignMonitoringStatusCount>> ObterCampanhasPorStatusMonitoramentoAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim);
    }
}