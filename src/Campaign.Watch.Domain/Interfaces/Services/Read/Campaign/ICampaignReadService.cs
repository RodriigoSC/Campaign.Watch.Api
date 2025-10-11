using Campaign.Watch.Domain.Entities.Read.Campaign;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Read.Campaign
{
    /// <summary>
    /// Define a interface para o serviço de leitura de dados de campanha de uma fonte externa.
    /// </summary>
    public interface ICampaignReadService
    {
        /// <summary>
        /// Busca as campanhas de um cliente em sua respectiva base de dados de origem.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente a ser consultado.</param>
        /// <returns>Uma coleção de campanhas lidas da fonte de dados.</returns>
        Task<IEnumerable<CampaignRead>> GetCampaignsByClient(string dbName);

        /// <summary>
        /// Busca as execuções de uma campanha específica na base de dados de origem do cliente.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente.</param>
        /// <param name="campaignId">O ID da campanha para a qual as execuções serão buscadas.</param>
        /// <returns>Uma coleção de execuções da campanha especificada.</returns>
        Task<IEnumerable<ExecutionRead>> GetExecutionsByCampaign(string dbName, string campaignId);

        /// <summary>
        /// Busca uma única campanha pelo seu ID na base de dados de origem do cliente.
        /// </summary>
        /// <param name="dbName">O nome do banco de dados do cliente.</param>
        /// <param name="campaignId">O ID da campanha a ser buscada.</param>
        /// <returns>A entidade da campanha lida da fonte de dados, ou nulo se não encontrada.</returns>
        Task<CampaignRead> GetCampaignById(string dbName, string campaignId);
    }
}