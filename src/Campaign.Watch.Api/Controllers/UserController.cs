using Campaign.Watch.Application.Dtos.Users;
using Campaign.Watch.Application.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Campaign.Watch.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApp;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserApplication userApp, ILogger<UserController> logger)
        {
            _userApp = userApp;
            _logger = logger;
        }

        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Tentativa de login para {Email}", request.Email);
            var response = await _userApp.LoginAsync(request);

            if (response == null)
            {
                _logger.LogWarning("Falha na autenticação para {Email}", request.Email);
                return Unauthorized("Usuário ou senha inválidos.");
            }

            return Ok(response);
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var userId = GetUserIdFromToken();
            _logger.LogInformation("Buscando configurações para o usuário {UserId}", userId);

            var settings = await _userApp.GetSettingsAsync(userId);

            if (settings == null)
            {
                return NotFound("Configurações do usuário não encontradas.");
            }

            return Ok(settings);
        }
        
        [HttpPut("settings/profile")]
        public async Task<IActionResult> UpdateProfileSettings([FromBody] UpdateProfileSettingsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromToken();
            _logger.LogInformation("Atualizando perfil do usuário {UserId}", userId);

            var success = await _userApp.UpdateProfileSettingsAsync(userId, request);

            if (!success)
            {
                return BadRequest("Não foi possível salvar as configurações de perfil.");
            }

            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userApp.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newUser = await _userApp.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetAllUsers), new { id = newUser.Id }, newUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _userApp.UpdateUserAsAdminAsync(id, request);
            if (!success)
            {
                return NotFound("Usuário não encontrado ou falha ao atualizar.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _userApp.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound("Usuário não encontrado.");
            }
            return NoContent();
        } 

        private string GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("ID do usuário não encontrado no token.");
            }

            return userIdClaim;
        }
    }
}