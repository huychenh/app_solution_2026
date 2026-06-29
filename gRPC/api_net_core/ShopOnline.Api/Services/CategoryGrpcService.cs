using Grpc.Core;
using ShopOnline.Api;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShopOnline.Api.Services
{
    // Inherits from the Base class auto-generated from category.proto
    public class CategoryGrpcService : CategoryGrpc.CategoryGrpcBase
    {
        // Inject your existing business service that connects to the real SQL DB
        // Replace 'ICategoryService' with your actual service interface name if it differs
        private readonly ICategoryService _categoryService;

        public CategoryGrpcService(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Handles the gRPC request to fetch all categories from the SQL database
        public override async Task<CategoryListResponse> GetCategoryList(EmptyRequest request, ServerCallContext context)
        {
            var response = new CategoryListResponse();

            // 1. Get real data from your service
            var categoriesDto = await _categoryService.GetAllAsync();

            // 2. Map safely to gRPC response to prevent Nullable exceptions
            foreach (var item in categoriesDto)
            {
                response.Categories.Add(new CategoryResponse
                {
                    Id = item.Id,
                    Name = item.Name ?? "",
                    Description = item.Description ?? "",

                    // Safe mapping for Nullable DateTime fields without using .Value directly
                    CreatedDate = item.CreatedDate != null
                        ? item.CreatedDate.ToString("yyyy-MM-dd HH:mm")
                        : "",

                    UpdatedDate = item.UpdatedDate != null
                        ? item.UpdatedDate.ToString("yyyy-MM-dd HH:mm")
                        : "",

                    CreatedBy = item.CreatedBy ?? "System",
                    UpdatedBy = item.UpdatedBy ?? "System",
                    IsActived = item.IsActived, // Mapped directly to protobuf bool
                    IsDeleted = item.IsDeleted  // Mapped directly to protobuf bool
                });
            }

            return response;
        }
    }
}