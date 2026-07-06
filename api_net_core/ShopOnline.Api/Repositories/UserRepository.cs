using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Models;
using ShopOnline.Api.Data;

namespace ShopOnline.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FindAsync(id);

        public async Task AddAsync(User User)
        {
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, User User)
        {
            if (User.Id != id)
                throw new ArgumentException("User ID mismatch");
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(User);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var User = await _context.Users.FindAsync(id);
            if (User == null) return false;

            _context.Users.Remove(User);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}

