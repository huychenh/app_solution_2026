using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Helpers;
using ShopOnline.Api.Models;

namespace ShopOnline.Api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _context.Categories.ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories.FindAsync(id);

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            if (category.Id != id)
                throw new ArgumentException("Category ID mismatch");
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Category>> GetAllByKeywordAsync(string? keyword, int page, int pageSize)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string searchKeyword = keyword.Trim();
                query = query.Where(c => c.Name.Contains(searchKeyword)
                                      || (c.Description != null && c.Description.Contains(searchKeyword)));
            }

            int totalCount = await query.CountAsync();

            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}
