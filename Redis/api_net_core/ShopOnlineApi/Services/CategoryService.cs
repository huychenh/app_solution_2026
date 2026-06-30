using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using ShopOnline.Api.Models;
using ShopOnline.Api.Repositories;
using ShopOnline.Common;
using System.Text.Json;

namespace ShopOnline.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        // Using constants prevents typos and makes it easier to update keys globally
        private const string ALL_CATEGORIES_CACHE_KEY = "all_categories";
        private const string SINGLE_CATEGORY_CACHE_PREFIX = "category_";

        public CategoryService(ICategoryRepository repo, IMapper mapper, IDistributedCache cache)
        {
            _repo = repo;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
        {
            // Cleaned: Used the constant instead of a hardcoded string
            string cacheKey = ALL_CATEGORIES_CACHE_KEY;

            // 1. Try to get data from Redis Cache
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // If cache exists, deserialize the JSON string back to List Object and return immediately
                return JsonSerializer.Deserialize<IEnumerable<CategoryReadDto>>(cachedData)!;
            }

            // 2. If cache miss, fetch data from the Database via Repository
            var categories = await _repo.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<CategoryReadDto>>(categories);

            // 3. Save the fetched data back to Redis (Set Time-To-Live to 15 minutes)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            var serializedData = JsonSerializer.Serialize(dtos);
            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

            return dtos;
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id)
        {
            // Cleaned: Used the constant prefix instead of hardcoded "category_"
            string cacheKey = $"{SINGLE_CATEGORY_CACHE_PREFIX}{id}";

            // 1. Try to find this specific record in Redis first
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // If cache hits, return the data immediately
                return JsonSerializer.Deserialize<CategoryReadDto>(cachedData);
            }

            // 2. If cache miss, find the record in the Database
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return null;

            var dto = _mapper.Map<CategoryReadDto>(category);

            // 3. Save this individual record to Redis for subsequent requests
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

            return dto;
        }

        public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _repo.AddAsync(category);

            // Note: In production, you should clear cache here to avoid stale data
            // await _cache.RemoveAsync(ALL_CATEGORIES_CACHE_KEY);

            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return false;

            _mapper.Map(dto, existing);
            existing.UpdatedDate = DateTime.UtcNow;

            var result = await _repo.UpdateAsync(id, existing);

            // If database update is successful, invalidate the cache
            if (result)
            {
                // Clear the list cache because data has changed
                await _cache.RemoveAsync(ALL_CATEGORIES_CACHE_KEY);

                // Clear the specific single item cache as well
                await _cache.RemoveAsync($"{SINGLE_CATEGORY_CACHE_PREFIX}{id}");
            }

            return result;
        }

        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}