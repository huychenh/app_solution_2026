using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using ShopOnline.Api.Helpers;
using ShopOnline.Api.Services;
using ShopOnline.Common;
using ShopOnline.Common.Messages;
using System.Text;
using System.Text.Json;

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

        // GET: api/categories/list        
        [Authorize]        
        [HttpGet("search")]        
        public async Task<ActionResult<PagedResult<CategoryReadDto>>> GetListByKeyword(
            [FromQuery] string? keyword = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await service.GetAllByKeywordAsync(keyword, page, pageSize);
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
            // 1. Your existing logic: Save category via service layer
            var createdDto = await service.CreateAsync(dto);

            // 2. NEW: Publish event to RabbitMQ Topic Exchange asynchronously
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                // 🛠️ FIX 1: Declare a Topic Exchange instead of a hardcoded queue
                string exchangeName = "shop-online-exchange";
                await channel.ExchangeDeclareAsync(exchange: exchangeName,
                                                   type: ExchangeType.Topic,
                                                   durable: true);

                // Map data from your created DTO to the shared message contract
                var categoryEvent = new CategoryCreatedEvent
                {
                    Id = createdDto.Id,
                    Name = createdDto.Name,
                    Description = createdDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    IsActived = createdDto.IsActived,
                    CreatedBy = createdDto.CreatedBy,
                    UpdatedBy = createdDto.UpdatedBy,
                };

                var messageJson = JsonSerializer.Serialize(categoryEvent);
                var body = Encoding.UTF8.GetBytes(messageJson);

                // 🛠️ FIX 2: Publish to the TOPIC EXCHANGE with a structured routing key
                // We use "category.created" so any consumer binding to "category.*" can receive it
                await channel.BasicPublishAsync(exchange: exchangeName,
                                                routingKey: "category.created",
                                                mandatory: true,
                                                body: body);
            }
            catch (Exception ex)
            {
                // Log the exception here if RabbitMQ server is down, 
                // ensuring the main API response doesn't fail if messaging fails.
                Console.WriteLine($"[RabbitMQ Error] Failed to publish message: {ex.Message}");
            }

            // 3. Your existing logic: Return the standard 201 Created response
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
