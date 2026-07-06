using ShopOnline.Api.Models;

namespace ShopOnline.Api.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task<bool> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
    }
}
