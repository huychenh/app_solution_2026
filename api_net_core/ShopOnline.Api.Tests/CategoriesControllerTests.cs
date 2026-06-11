using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ShopOnline.Api.Controllers;
using ShopOnline.Api.Services;
using ShopOnline.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ShopOnline.Api.Tests
{
    public class CategoriesControllerTests
    {
        private readonly ICategoryService _mockService;
        private readonly CategoriesController _controller;

        // Constructor
        public CategoriesControllerTests()
        {
            _mockService = Substitute.For<ICategoryService>();
            _controller = new CategoriesController(_mockService);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            // Arrange
            var categories = new List<CategoryReadDto>
            {
                new() { Id = 1, Name = "Cat1" },
                new() { Id = 2, Name = "Cat2" }
            };
            _mockService.GetAllAsync().Returns(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<CategoryReadDto>>(ok.Value);
            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            // Arrange
            var category = new CategoryReadDto { Id = 1, Name = "Cat1" };
            _mockService.GetByIdAsync(1).Returns(category);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsType<CategoryReadDto>(ok.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            _ = _mockService.GetByIdAsync(9).Returns((CategoryReadDto?)null);

            // Act
            var result = await _controller.GetById(9);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CategoryCreateDto { Name = "NewCat" };
            var createdCategory = new CategoryReadDto { Id = 5, Name = "NewCat" };

            _mockService.CreateAsync(dto).Returns(createdCategory);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var model = Assert.IsType<CategoryReadDto>(created.Value);
            Assert.Equal(5, model.Id);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var dto = new CategoryUpdateDto { Name = "Updated" };
            _mockService.UpdateAsync(1, dto).Returns(true);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var dto = new CategoryUpdateDto { Name = "Updated" };
            _mockService.UpdateAsync(9, dto).Returns(false);

            // Act
            var result = await _controller.Update(9, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            _mockService.DeleteAsync(1).Returns(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            _mockService.DeleteAsync(9).Returns(false);

            // Act
            var result = await _controller.Delete(9);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}