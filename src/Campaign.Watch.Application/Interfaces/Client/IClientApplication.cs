using Campaign.Watch.Application.Dtos.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Client
{
    public interface IClientApplication
    {
        Task<ClientResponse> CreateClientAsync(SaveClientRequest clientDto);
        Task<IEnumerable<ClientResponse>> GetAllClientsAsync();
        Task<ClientResponse> GetClientByIdAsync(string id);
        Task<ClientResponse> GetClientByNameAsync(string clientName);
        Task<bool> UpdateClientAsync(string id, SaveClientRequest clientDto);
        Task<bool> DeleteClientAsync(string id);
    }
}
