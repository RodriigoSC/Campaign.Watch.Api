using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Repositories.Campaign
{
    public interface ICampaignRepository
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
        Task<int> ContarCampanhasPorClienteAsync(string nomeCliente);
        Task<IEnumerable<CampaignEntity>> ObterCampanhasParaMonitorarAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasComErrosDeIntegracaoAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasComExecucaoAtrasadaAsync();
        Task<IEnumerable<CampaignEntity>> ObterCampanhasMonitoradasComSucessoAsync();

        
        Task<long> ContarCampanhasFiltradasAsync(string clientName, string monitoringStatus, bool? hasErrors, DateTime? dataInicio, DateTime? dataFim);               
        Task<IEnumerable<CampaignEntity>> ObterCampanhasPaginadasAsync(int pagina, int tamanhoPagina, string clientName, string monitoringStatus,
            bool? hasErrors, DateTime? dataInicio, DateTime? dataFim);
    }
}