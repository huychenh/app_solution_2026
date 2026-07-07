using ShopOnline.Api.Helpers;
using ShopOnline.Api.Models;
using ShopOnline.Common;

namespace ShopOnline.Api.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDto>> GetAllAsync();

        //Task<IEnumerable<CategoryReadDto>> GetAllByKeywordAsync(string? keyword, int page, int pageSize);
        Task<PagedResult<CategoryReadDto>> GetAllByKeywordAsync(string? keyword, int page, int pageSize);

        Task<CategoryReadDto?> GetByIdAsync(int id);

        Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto);

        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
