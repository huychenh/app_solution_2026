using MediatR;

namespace ShopOnline.Api.RequestHandlers.Categories;

public record CreateCategoryCommand(string Name, string Description) : IRequest<int>;
