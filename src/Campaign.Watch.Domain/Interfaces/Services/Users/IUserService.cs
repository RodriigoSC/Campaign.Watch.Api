using Campaign.Watch.Domain.Entities.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Services.Users
{
    public interface IUserService
    {
        Task<(UserEntity user, string token)?> AuthenticateAsync(string email, string password);
        Task<UserEntity> GetByIdAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, string name, string email, string phone);
        Task<bool> UpdateSettingsAsync(string userId, UserSettings settings);

        Task<IEnumerable<UserEntity>> GetAllUsersAsync();
        Task<UserEntity> CreateUserAsync(string name, string email, string plainTextPassword, string role);
        Task<bool> UpdateUserAsAdminAsync(string userId, string name, string email, string role, bool isActive, string phone);
        Task<bool> DeleteUserAsync(string userId);
        Task SeedAdminUserAsync();
    }
}
