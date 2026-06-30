using ShopOnline.Api.Models;

namespace ShopOnline.Api.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task<bool> UpdateAsync(int id, Category category);
        Task<bool> DeleteAsync(int id);
    }
}
