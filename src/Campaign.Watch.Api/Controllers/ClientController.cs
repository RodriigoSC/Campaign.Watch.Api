using Campaign.Watch.Application.Dtos.Client;
using Campaign.Watch.Application.Interfaces.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    /// <summary>
    /// Controller da API para gerenciar operações relacionadas a clientes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientApplication _clientApplication;

        /// <summary>
        /// Inicializa uma nova instância do ClientController.
        /// </summary>
        /// <param name="clientApplication">A camada de aplicação do cliente a ser injetada.</param>
        public ClientController(IClientApplication clientApplication)
        {
            _clientApplication = clientApplication;
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="clientDto">Os dados do cliente a ser criado.</param>
        /// <returns>O cliente recém-criado.</returns>
        /// <response code="201">Retorna o cliente recém-criado.</response>
        /// <response code="400">Se os dados fornecidos forem inválidos ou se já existir um cliente com o mesmo nome.</response>
        /// <response code="500">Se ocorrer um erro inesperado no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ClientResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateClient([FromBody] SaveClientRequest clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdClient = await _clientApplication.CreateClientAsync(clientDto);
                return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
            }
            catch (InvalidOperationException ex) // Exceção específica para nome duplicado
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao criar o cliente.");
            }
        }

        /// <summary>
        /// Obtém uma lista de todos os clientes.
        /// </summary>
        /// <returns>Uma lista de clientes.</returns>
        /// <response code="200">Retorna a lista de clientes.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientResponse>), 200)]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientApplication.GetAllClientsAsync();
            return Ok(clients);
        }

        /// <summary>
        /// Obtém um cliente específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do cliente.</param>
        /// <returns>Os dados do cliente.</returns>
        /// <response code="200">Retorna os dados do cliente encontrado.</response>
        /// <response code="404">Se o cliente não for encontrado.</response>
        [HttpGet("{id}", Name = "GetClientById")]
        [ProducesResponseType(typeof(ClientResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetClientById(string id)
        {
            var client = await _clientApplication.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        /// <summary>
        /// Obtém um cliente específico pelo seu nome.
        /// </summary>
        /// <param name="clientName">O nome do cliente.</param>
        /// <returns>Os dados do cliente.</returns>
        /// <response code="200">Retorna os dados do cliente encontrado.</response>
        /// <response code="404">Se o cliente não for encontrado.</response>
        [HttpGet("by-name/{clientName}")]
        [ProducesResponseType(typeof(ClientResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetClientByName(string clientName)
        {
            var client = await _clientApplication.GetClientByNameAsync(clientName);
            if (client == null)
            {
                return NotFound($"Cliente com o nome '{clientName}' não encontrado.");
            }
            return Ok(client);
        }

        /// <summary>
        /// Atualiza um cliente existente.
        /// </summary>
        /// <param name="id">O ID do cliente a ser atualizado.</param>
        /// <param name="clientDto">Os novos dados para o cliente.</param>
        /// <returns>Nenhum conteúdo.</returns>
        /// <response code="204">Se o cliente foi atualizado com sucesso.</response>
        /// <response code="400">Se os dados fornecidos forem inválidos.</response>
        /// <response code="404">Se o cliente não for encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClient(string id, [FromBody] SaveClientRequest clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _clientApplication.UpdateClientAsync(id, clientDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Deleta um cliente.
        /// </summary>
        /// <param name="id">O ID do cliente a ser deletado.</param>
        /// <returns>Nenhum conteúdo.</returns>
        /// <response code="204">Se o cliente foi deletado com sucesso.</response>
        /// <response code="404">Se o cliente não for encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClient(string id)
        {
            var success = await _clientApplication.DeleteClientAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}