using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Services;
using ShopOnline.Common;

namespace ShopOnline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService service) : ControllerBase
    {
        // GET: api/categories/list
        [Authorize]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/categories/getbyid/1
        [Authorize]
        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetById(int id)
        {
            var category = await service.GetByIdAsync(id);
            return category is null ? NotFound() : Ok(category);
        }

        // POST: api/categories/create
        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("create")]
        public async Task<ActionResult<CategoryReadDto>> Create(CategoryCreateDto dto)
        {
            var createdDto = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/categories/update/1
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            var result = await service.UpdateAsync(id, dto);
            return result ? NoContent() : NotFound();
        }

        // DELETE: api/categories/delete/1
        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
