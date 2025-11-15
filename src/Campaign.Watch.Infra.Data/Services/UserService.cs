using Campaign.Watch.Domain.Entities.Users;
using Campaign.Watch.Domain.Interfaces.Repositories.Users;
using Campaign.Watch.Domain.Interfaces.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(UserEntity user, string token)?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return null;

            if (!user.IsActive)
                throw new ArgumentException("Usuário desativado.");

            if (string.IsNullOrEmpty(user.PasswordHash))
                return null;

            var passwordMatches = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatches)
                return null;

            var token = GenerateJwtToken(user);

            return (user, token);
        }


        public async Task<UserEntity> GetByIdAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
            {
                throw new ArgumentException("ID de usuário inválido.");
            }
            return await _userRepository.GetByIdAsync(objectId);
        }

        public async Task<bool> UpdateProfileAsync(string userId, string name, string email, string phone)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return false;

            user.Name = name;
            user.Email = email;
            user.Phone = phone;

            return await _userRepository.UpdateAsync(user);
        }

        
        public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserEntity> CreateUserAsync(string name, string email, string plainTextPassword, string role)
        {
            if (await _userRepository.GetByEmailAsync(email) != null)
            {
                throw new InvalidOperationException("Um usuário com este e-mail já existe.");
            }

            var user = new UserEntity
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword),
                Role = role,
                IsActive = true
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> UpdateUserAsAdminAsync(string userId, string name, string email, string role, bool isActive, string phone)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return false;

            user.Name = name;
            user.Email = email;
            user.Role = role;
            user.IsActive = isActive;
            user.Phone = phone;

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
            {
                throw new ArgumentException("ID de usuário inválido.");
            }
            return await _userRepository.DeleteAsync(objectId);
        }

        public async Task SeedAdminUserAsync()
        {
            try
            {
                var adminCount = await _userRepository.CountByRoleAsync("Admin");
                if (adminCount > 0)
                {
                    _logger.LogInformation("Usuário 'Admin' já existe. Seeding ignorado.");
                    return;
                }

                _logger.LogWarning("NENHUM USUÁRIO 'Admin' ENCONTRADO. Criando usuário padrão...");

                var email = _configuration["DefaultAdmin:Email"];
                var password = _configuration["DefaultAdmin:Password"];

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    _logger.LogError("DefaultAdmin:Email ou DefaultAdmin:Password não configurados no appsettings.Development.json. Não é possível criar o usuário padrão.");
                    return;
                }

                await CreateUserAsync("Administrador Padrão", email, password, "Admin");

                _logger.LogInformation("Usuário 'Admin' padrão criado com sucesso. Email: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha crítica ao tentar criar o usuário 'Admin' padrão durante o seeding.");
            }
        }

        // --- Método Privado para Gerar o Token ---

        private string GenerateJwtToken(UserEntity user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurada.");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience não configurada.");
            var tokenExpiresString = _configuration["Jwt:TokenExpires"] ?? throw new InvalidOperationException("Jwt:TokenExpires não configurado.");

            if (!double.TryParse(tokenExpiresString, out double tokenExpiresHours))
            {
                throw new InvalidOperationException("Jwt:TokenExpires está em um formato inválido. Deve ser um número de horas (ex: 8).");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(tokenExpiresHours),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}