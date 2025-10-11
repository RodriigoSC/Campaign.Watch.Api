using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Client;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IClientRepository _clientRepository;

        public CampaignService(ICampaignRepository campaignRepository, IClientRepository clientRepository)
        {
            _campaignRepository = campaignRepository;
            _clientRepository = clientRepository;
        }

        public async Task<CampaignEntity> CriarCampanhaAsync(CampaignEntity entity)
        {
            var client = await _clientRepository.GetByNameAsync(entity.ClientName);
            if (client == null)
            {
                throw new ArgumentException($"O cliente especificado ('{entity.ClientName}') não existe.");
            }
            if (!client.IsActive)
            {
                throw new InvalidOperationException($"Não é possível criar campanhas para um cliente inativo.");
            }

            return await _campaignRepository.CriarCampanhaAsync(entity);
        }

        public async Task<bool> AtualizarCampanhaAsync(ObjectId id, CampaignEntity entity)
        {
            var client = await _clientRepository.GetByNameAsync(entity.ClientName);
            if (client == null)
            {
                throw new ArgumentException($"O cliente especificado ('{entity.ClientName}') não existe.");
            }

            entity.Id = id;

            return await _campaignRepository.AtualizarCampanhaAsync(id, entity);
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasAsync()
        {
            return await _campaignRepository.ObterTodasAsCampanhasAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorClienteAsync(string nomeCliente)
        {
            return await _campaignRepository.ObterTodasAsCampanhasPorClienteAsync(nomeCliente);
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorDataAsync(DateTime inicio, DateTime fim)
        {
            return await _campaignRepository.ObterTodasAsCampanhasPorDataAsync(inicio, fim);
        }

        public async Task<IEnumerable<CampaignEntity>> ObterTodasAsCampanhasPorClienteOuDataAsync(string nomeCliente, DateTime inicio, DateTime fim)
        {
            return await _campaignRepository.ObterTodasAsCampanhasPorClienteOuDataAsync(nomeCliente, inicio, fim);
        }

        public async Task<CampaignEntity> ObterCampanhaPorIdAsync(ObjectId id)
        {
            return await _campaignRepository.ObterCampanhaPorIdAsync(id);
        }

        public async Task<CampaignEntity> ObterCampanhaPorNomeAsync(string nomeCampanha)
        {
            return await _campaignRepository.ObterCampanhaPorNomeAsync(nomeCampanha);
        }

        public async Task<CampaignEntity> ObterCampanhaPorNumeroAsync(long numeroCampanha)
        {
            return await _campaignRepository.ObterCampanhaPorNumeroAsync(numeroCampanha);
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasAtivasAsync()
        {
            return await _campaignRepository.ObterCampanhasAtivasAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasPaginadasAsync(int pagina, int tamanhoPagina)
        {
            return await _campaignRepository.ObterCampanhasPaginadasAsync(pagina, tamanhoPagina);
        }

        public async Task<int> ContarCampanhasPorClienteAsync(string nomeCliente)
        {
            return await _campaignRepository.ContarCampanhasPorClienteAsync(nomeCliente);
        }

        public async Task<CampaignEntity> ObterCampanhaPorIdCampanhaAsync(string nomeCliente, string idCampanha)
        {
            return await _campaignRepository.ObterCampanhaPorIdCampanhaAsync(nomeCliente, idCampanha);
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasParaMonitorarAsync()
        {
            return await _campaignRepository.ObterCampanhasParaMonitorarAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasComErrosDeIntegracaoAsync()
        {
            return await _campaignRepository.ObterCampanhasComErrosDeIntegracaoAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasComExecucaoAtrasadaAsync()
        {
            return await _campaignRepository.ObterCampanhasComExecucaoAtrasadaAsync();
        }

        public async Task<IEnumerable<CampaignEntity>> ObterCampanhasMonitoradasComSucessoAsync()
        {
            return await _campaignRepository.ObterCampanhasMonitoradasComSucessoAsync();
        }

        public async Task<IEnumerable<CampaignStatusCount>> ObterCampanhasPorStatusAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim)
        {
            return await _campaignRepository.ObterCampanhasPorStatusAsync(nomeCliente, dataInicio, dataFim);
        }

        public async Task<IEnumerable<CampaignMonitoringStatusCount>> ObterCampanhasPorStatusMonitoramentoAsync(string nomeCliente, DateTime? dataInicio, DateTime? dataFim)
        {
            return await _campaignRepository.ObterCampanhasPorStatusMonitoramentoAsync(nomeCliente, dataInicio, dataFim);
        }
    }
}