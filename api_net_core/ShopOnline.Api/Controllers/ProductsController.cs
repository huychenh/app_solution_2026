using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Services;
using ShopOnline.Common;

namespace ShopOnline.Api.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService service) : ControllerBase
    {
        [Authorize]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<ProductReadDto>> GetById(int id)
        {
            var product = await service.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("create")]
        public async Task<ActionResult<ProductReadDto>> Create(ProductCreateDto dto)
        {
            var createdDto = await service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }

        [Authorize(Policy = "RequireAdmin")]        
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var result = await service.UpdateAsync(id, dto);
            return result ? NoContent() : NotFound();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }

}
