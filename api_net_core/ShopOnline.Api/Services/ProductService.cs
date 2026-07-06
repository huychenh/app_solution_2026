using AutoMapper;
using ShopOnline.Api.Models;
using ShopOnline.Api.Repositories;
using ShopOnline.Common;

namespace ShopOnline.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductReadDto>>(products);
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductReadDto>(product);
        }

        public async Task<ProductReadDto> AddAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            return _mapper.Map<ProductReadDto>(product);
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return false;

            _mapper.Map(dto, existing); // update dto field to existing entity
            return await _repo.UpdateAsync(id, existing);
        }


        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
