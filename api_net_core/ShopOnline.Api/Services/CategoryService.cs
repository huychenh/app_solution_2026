using AutoMapper;
using ShopOnline.Api.Helpers;
using ShopOnline.Api.Models;
using ShopOnline.Api.Repositories;
using ShopOnline.Common;

namespace ShopOnline.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
        }

        public async Task<PagedResult<CategoryReadDto>> GetAllByKeywordAsync(string? keyword, int page, int pageSize)
        {
            // 1. Fetch the paginated entity model block from the repository layer
            var pagedCategories = await _repo.GetAllByKeywordAsync(keyword, page, pageSize);

            // 2. Map only the generic item collection and preserve the absolute total count properties
            return new PagedResult<CategoryReadDto>
            {
                Items = _mapper.Map<IEnumerable<CategoryReadDto>>(pagedCategories.Items),
                TotalCount = pagedCategories.TotalCount
            };
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = _mapper.Map<Category>(dto);            
            await _repo.AddAsync(category);
            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return false;

            _mapper.Map(dto, existing);
            existing.UpdatedDate = DateTime.UtcNow;

            return await _repo.UpdateAsync(id, existing);
        }

        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);

    }
}
