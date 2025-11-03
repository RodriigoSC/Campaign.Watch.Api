using Campaign.Watch.Domain.Entities.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Client
{
    /// <summary>
    /// Define a interface para o serviço de clientes, especificando as operações de negócio.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="client">A entidade do cliente a ser criada.</param>
        /// <returns>A entidade do cliente após a criação.</returns>
        Task<ClientEntity> CreateClientAsync(ClientEntity client);

        /// <summary>
        /// Busca todos os clientes cadastrados.
        /// </summary>
        /// <returns>Uma coleção com todos os clientes.</returns>
        Task<IEnumerable<ClientEntity>> GetAllClientsAsync();

        /// <summary>
        /// Busca um cliente pelo seu ID.
        /// </summary>
        /// <param name="id">O ID (string) do cliente.</param>
        /// <returns>A entidade do cliente, ou nulo se não for encontrado.</returns>
        Task<ClientEntity> GetClientByIdAsync(string id);

        /// <summary>
        /// Busca um cliente pelo seu nome.
        /// </summary>
        /// <param name="clientName">O nome do cliente.</param>
        /// <returns>A entidade do cliente, ou nulo se não for encontrado.</returns>
        Task<ClientEntity> GetClientByNameAsync(string clientName);

        /// <summary>
        /// Atualiza um cliente existente.
        /// </summary>
        /// <param name="id">O ID do cliente a ser atualizado.</param>
        /// <param name="client">A entidade com os dados atualizados.</param>
        /// <returns>Retorna true se a atualização foi bem-sucedida, caso contrário, false.</returns>
        Task<bool> UpdateClientAsync(string id, ClientEntity client);

        /// <summary>
        /// Deleta um cliente com base em seu ID.
        /// </summary>
        /// <param name="id">O ID do cliente a ser deletado.</param>
        /// <returns>Retorna true se a exclusão foi bem-sucedida, caso contrário, false.</returns>
        Task<bool> DeleteClientAsync(string id);
    }
}