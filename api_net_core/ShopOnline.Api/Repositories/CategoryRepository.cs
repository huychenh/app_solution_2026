using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
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

        public async Task<IEnumerable<Category>> GetAllByKeywordAsync(string? keyword = null)
        {
            // 1. Prepare the base query from the DbContext (deferred execution)
            var query = _context.Categories.AsQueryable();

            // 2. Apply filtering conditionally if a keyword is provided
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string searchKeyword = keyword.Trim();

                // Filter categories where Name OR Description contains the keyword
                query = query.Where(c => c.Name.Contains(searchKeyword)
                                      || (c.Description != null && c.Description.Contains(searchKeyword)));
            }

            // 3. Execute the SQL query asynchronously against the database
            return await query.ToListAsync();
        }
    }
}
