using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Services;
using ShopOnline.Common;
using ShopOnline.Api.Models;
using ShopOnline.Api.RequestHandlers.Categories;

namespace ShopOnline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Using Primary Constructor to inject both legacy service and new Mediator
    public class CategoriesController(ICategoryService service, IMediator mediator) : ControllerBase
    {
        // GET: api/categories/list
        //[Authorize]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CategoryResultDto>>> GetAll()
        {
            // Migrated to MediatR Query successfully
            var result = await mediator.Send(new GetCategoryListQuery());
            return Ok(result);
        }

        // GET: api/categories/getbyid/1
        [Authorize]
        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetById(int id)
        {
            // Keeping legacy service layer for now
            var category = await service.GetByIdAsync(id);
            return category is null ? NotFound() : Ok(category);
        }

        // POST: api/categories/create        
        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("create")]
        public async Task<ActionResult<int>> Create(CreateCategoryCommand command)
        {
            // Migrated to MediatR Command successfully
            // All DB persistence and RabbitMQ publishing logic should be handled inside CreateCategoryCommandHandler
            var categoryId = await mediator.Send(command);

            // Return the created resource ID adhering to RESTful standards
            return CreatedAtAction(nameof(GetById), new { id = categoryId }, categoryId);
        }

        // PUT: api/categories/update/1
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            // Keeping legacy service layer for now
            var result = await service.UpdateAsync(id, dto);
            return result ? NoContent() : NotFound();
        }

        // DELETE: api/categories/delete/1
        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Keeping legacy service layer for now
            var result = await service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}