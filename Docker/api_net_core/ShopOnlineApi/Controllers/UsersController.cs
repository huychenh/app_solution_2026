using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Services;
using ShopOnline.Common;

namespace ShopOnline.Api.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService service) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<UserReadDto>> GetById(int id)
        {
            var User = await service.GetByIdAsync(id);
            return User is null ? NotFound() : Ok(User);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("create")]
        public async Task<ActionResult<UserReadDto>> Create(UserCreateDto dto)
        {
            var createdDto = await service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
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
