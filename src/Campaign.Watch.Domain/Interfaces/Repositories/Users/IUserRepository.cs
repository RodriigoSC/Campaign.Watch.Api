using Campaign.Watch.Domain.Entities.Users;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Interfaces.Repositories.Users
{
    public interface IUserRepository
    {
        Task<UserEntity> GetByIdAsync(ObjectId id);
        Task<UserEntity> GetByEmailAsync(string email);
        Task<UserEntity> CreateAsync(UserEntity entity);
        Task<bool> UpdateAsync(UserEntity entity);
        Task<IEnumerable<UserEntity>> GetAllAsync();
        Task<bool> DeleteAsync(ObjectId id);
        Task<long> CountByRoleAsync(string role);
    }
}
