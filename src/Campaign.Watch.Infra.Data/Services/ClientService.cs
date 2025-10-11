using Campaign.Watch.Domain.Entities.Client;
using Campaign.Watch.Domain.Interfaces.Repositories.Client;
using Campaign.Watch.Domain.Interfaces.Services.Client;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Campaign.Watch.Infra.Data.Services
{
    /// <summary>
    /// Implementação do serviço de clientes, contendo as regras de negócio para operações de cliente.
    /// </summary>
    public class ClientService : IClientService
    {
        /// <summary>
        /// O repositório de clientes para acesso aos dados.
        /// </summary>
        private readonly IClientRepository _clientRepository;

        /// <summary>
        /// Inicializa uma nova instância da classe ClientService.
        /// </summary>
        /// <param name="clientRepository">O repositório de clientes a ser injetado.</param>
        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        /// <summary>
        /// Cria um novo cliente após validar que não existe outro cliente com o mesmo nome.
        /// </summary>
        /// <param name="client">A entidade do cliente a ser criada.</param>
        /// <returns>A entidade do cliente após a criação.</returns>
        /// <exception cref="InvalidOperationException">Lançada se já existir um cliente com o mesmo nome.</exception>
        public async Task<ClientEntity> CreateClientAsync(ClientEntity client)
        {
            var existingClient = await _clientRepository.GetByNameAsync(client.Name);
            if (existingClient != null)
            {
                throw new InvalidOperationException("Já existe um cliente com este nome.");
            }
            return await _clientRepository.CreateAsync(client);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClientEntity>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        /// <summary>
        /// Busca um cliente pelo seu ID, após validar e converter a string de ID para ObjectId.
        /// </summary>
        /// <param name="id">O ID (string) do cliente.</param>
        /// <returns>A entidade do cliente, ou nulo se o ID for inválido ou o cliente não for encontrado.</returns>
        public async Task<ClientEntity> GetClientByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            return await _clientRepository.GetByIdAsync(objectId);
        }

        /// <inheritdoc />
        public async Task<ClientEntity> GetClientByNameAsync(string clientName)
        {
            return await _clientRepository.GetByNameAsync(clientName);
        }

        /// <summary>
        /// Atualiza um cliente existente, após validar e converter a string de ID para ObjectId.
        /// </summary>
        /// <param name="id">O ID do cliente a ser atualizado.</param>
        /// <param name="client">A entidade com os dados atualizados.</param>
        /// <returns>Retorna true se o ID for válido e a atualização bem-sucedida; caso contrário, false.</returns>
        public async Task<bool> UpdateClientAsync(string id, ClientEntity client)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            return await _clientRepository.UpdateAsync(objectId, client);
        }

        /// <summary>
        /// Deleta um cliente com base em seu ID, após validar e converter a string de ID para ObjectId.
        /// </summary>
        /// <param name="id">O ID do cliente a ser deletado.</param>
        /// <returns>Retorna true se o ID for válido e a exclusão bem-sucedida; caso contrário, false.</returns>
        public async Task<bool> DeleteClientAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            return await _clientRepository.DeleteAsync(objectId);
        }
    }
}