using ShopOnline.Common;

namespace ShopOnline.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(int id);
        Task<UserReadDto> AddAsync(UserCreateDto dto);
        Task<bool> UpdateAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
