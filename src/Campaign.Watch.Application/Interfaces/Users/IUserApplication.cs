using Campaign.Watch.Application.Dtos.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Users
{
    public interface IUserApplication
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<SettingsDto> GetSettingsAsync(string userId);
        Task<bool> UpdateProfileSettingsAsync(string userId, UpdateProfileSettingsRequest request);
        Task<IEnumerable<UserSummaryResponse>> GetAllUsersAsync();
        Task<UserSummaryResponse> CreateUserAsync(CreateUserRequest request);
        Task<bool> UpdateUserAsAdminAsync(string userId, AdminUpdateUserRequest request);
        Task<bool> DeleteUserAsync(string userId);
    }
}
