using AutoMapper;
using Campaign.Watch.Application.Dtos.Users;
using Campaign.Watch.Application.Interfaces.Users;
using Campaign.Watch.Domain.Entities.Users;
using Campaign.Watch.Domain.Interfaces.Services.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Users
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserApplication(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var authResult = await _userService.AuthenticateAsync(request.Email, request.Password);

            if (authResult == null)
            {
                return null;
            }

            var (user, token) = authResult.Value;

            var userInfo = _mapper.Map<UserInfo>(user);

            return new LoginResponse
            {
                Token = token,
                User = userInfo
            };
        }

        public async Task<SettingsDto> GetSettingsAsync(string userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return null;

            var settingsDto = _mapper.Map<SettingsDto>(user);

            return settingsDto;
        }

        public async Task<bool> UpdateProfileSettingsAsync(string userId, UpdateProfileSettingsRequest request)
        {
            return await _userService.UpdateProfileAsync(userId, request.Name, request.Email, request.Phone);
        }

        public async Task<IEnumerable<UserSummaryResponse>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserSummaryResponse>>(users);
        }

        public async Task<UserSummaryResponse> CreateUserAsync(CreateUserRequest request)
        {
            var newUser = await _userService.CreateUserAsync(request.Name, request.Email, request.Password, request.Role);
            return _mapper.Map<UserSummaryResponse>(newUser);
        }

        public async Task<bool> UpdateUserAsAdminAsync(string userId, AdminUpdateUserRequest request)
        {
            return await _userService.UpdateUserAsAdminAsync(userId, request.Name, request.Email, request.Role, request.IsActive, request.Phone);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            return await _userService.DeleteUserAsync(userId);
        }
    }
}
