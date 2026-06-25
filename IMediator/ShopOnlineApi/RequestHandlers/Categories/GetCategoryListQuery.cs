using MediatR;
using ShopOnline.Api.Models;

namespace ShopOnline.Api.RequestHandlers.Categories;

public record GetCategoryListQuery : IRequest<List<CategoryResultDto>>;