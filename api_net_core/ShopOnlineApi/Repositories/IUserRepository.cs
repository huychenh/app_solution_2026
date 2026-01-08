using ShopOnline.Api.Models;

namespace ShopOnline.Api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User User);
        Task<bool> UpdateAsync(int id, User User);
        Task<bool> DeleteAsync(int id);
    }
}
