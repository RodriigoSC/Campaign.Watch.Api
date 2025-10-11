using Campaign.Watch.Domain.Entities.Client;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Repositories.Client
{
    /// <summary>
    /// Define a interface para o repositório de clientes, especificando os métodos de acesso a dados.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Cria um novo cliente no banco de dados.
        /// </summary>
        /// <param name="client">A entidade do cliente a ser criada.</param>
        /// <returns>A entidade do cliente após ser criada.</returns>
        Task<ClientEntity> CreateAsync(ClientEntity client);

        /// <summary>
        /// Obtém todos os clientes cadastrados.
        /// </summary>
        /// <returns>Uma coleção de todos os clientes.</returns>
        Task<IEnumerable<ClientEntity>> GetAllAsync();

        /// <summary>
        /// Obtém um cliente específico pelo seu ObjectId.
        /// </summary>
        /// <param name="id">O ObjectId do cliente.</param>
        /// <returns>A entidade do cliente correspondente ao ID, ou nulo se não encontrado.</returns>
        Task<ClientEntity> GetByIdAsync(ObjectId id);

        /// <summary>
        /// Obtém um cliente específico pelo seu nome.
        /// </summary>
        /// <param name="clientName">O nome do cliente.</param>
        /// <returns>A entidade do cliente correspondente ao nome, ou nulo se não encontrado.</returns>
        Task<ClientEntity> GetByNameAsync(string clientName);

        /// <summary>
        /// Atualiza um cliente existente com base em seu ID.
        /// </summary>
        /// <param name="id">O ObjectId do cliente a ser atualizado.</param>
        /// <param name="client">A entidade com os dados atualizados.</param>
        /// <returns>Retorna true se a atualização foi bem-sucedida, caso contrário, false.</returns>
        Task<bool> UpdateAsync(ObjectId id, ClientEntity client);

        /// <summary>
        /// Deleta um cliente com base em seu ID.
        /// </summary>
        /// <param name="id">O ObjectId do cliente a ser deletado.</param>
        /// <returns>Retorna true se a exclusão foi bem-sucedida, caso contrário, false.</returns>
        Task<bool> DeleteAsync(ObjectId id);
    }
}