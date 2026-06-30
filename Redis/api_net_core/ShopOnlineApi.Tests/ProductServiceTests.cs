using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ShopOnline.Api.Helpers;
using ShopOnline.Api.Data;
using ShopOnline.Api.Repositories;
using ShopOnline.Common;
using ShopOnline.Api.Services;
using ShopOnline.Api.Models;

public class ProductServiceTests
{
    private readonly IMapper _mapper;

    public ProductServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();
    }

    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddAsync_Should_Add_Product_To_Database()
    {
        var context = CreateInMemoryDbContext();

        // 🧹 Clear any existing data (in case reusing context)
        context.Products.RemoveRange(context.Products);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);
        var service = new ProductService(repository, _mapper);

        var createDto = new ProductCreateDto
        {
            Name = "Test Product",
            Price = 100,
            Quantity = 5,
            Description = "Unit test"
        };

        var result = await service.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Single(await service.GetAllAsync());
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Expected_Number_Of_Products()
    {
        // Arrange
        var context = CreateInMemoryDbContext();

        // 🧹 Clear any existing data (in case reusing context)
        context.Products.RemoveRange(context.Products);
        await context.SaveChangesAsync();

        // 🧪 Add test data directly to DbContext
        context.Products.AddRange(
            new Product { Name = "P1", Price = 10, Quantity = 1, Description = "Test" },
            new Product { Name = "P2", Price = 20, Quantity = 2, Description = "Test" }
        );
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);
        var service = new ProductService(repository, _mapper);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_When_Exists()
    {
        var context = CreateInMemoryDbContext();
        var repository = new ProductRepository(context);
        var service = new ProductService(repository, _mapper);

        var product = new Product { Name = "Product A", Price = 50, Quantity = 2, Description = "Some desc" };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = await service.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Product A", result?.Name);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Product_When_Exists()
    {
        var context = CreateInMemoryDbContext();
        var repository = new ProductRepository(context);
        var service = new ProductService(repository, _mapper);

        var product = new Product { Name = "Old", Price = 10, Quantity = 1, Description = "desc" };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var updateDto = new ProductUpdateDto
        {
            Name = "Updated",
            Price = 99,
            Quantity = 3,
            Description = "updated"
        };

        var result = await service.UpdateAsync(product.Id, updateDto);

        Assert.True(result);

        var updated = await context.Products.FindAsync(product.Id);
        Assert.Equal("Updated", updated?.Name);
        Assert.Equal(99, updated?.Price);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Product_When_Exists()
    {
        var context = CreateInMemoryDbContext();
        var repository = new ProductRepository(context);
        var service = new ProductService(repository, _mapper);

        var product = new Product { Name = "ToDelete", Price = 50, Quantity = 1, Description = "desc" };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = await service.DeleteAsync(product.Id);

        Assert.True(result);
        var exists = await context.Products.FindAsync(product.Id);
        Assert.Null(exists);
    }
}
