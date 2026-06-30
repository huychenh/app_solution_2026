using MediatR;
using ShopOnline.Api.Models;
using ShopOnline.Api.Services;

namespace ShopOnline.Api.RequestHandlers.Categories;

public class GetCategoryListQueryHandler(ICategoryService service) : IRequestHandler<GetCategoryListQuery, List<CategoryResultDto>>
{
    public async Task<List<CategoryResultDto>> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
    {
        // Fetch real data from your legacy service layer
        var categories = await service.GetAllAsync();

        // Map CategoryReadDto (or your legacy Entity) to CategoryResultDto
        return categories.Select(c => new CategoryResultDto(c.Id, c.Name, c.Description)).ToList();
    }
}