using AutoMapper;
using ShopOnline.Api.Models;
using ShopOnline.Common;

namespace ShopOnline.Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Product mappings
            CreateMap<Product, ProductReadDto>();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            //User mappings
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();

            //Category mappings
            CreateMap<Category, CategoryReadDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
        }
    }
}
