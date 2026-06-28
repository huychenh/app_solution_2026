using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using ShopOnline.Api.Services;
using ShopOnline.Common;
using ShopOnline.Common.Messages;
using System.Text;
using System.Text.Json;

namespace ShopOnline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService service, IConfiguration configuration, HttpClient httpClient) : ControllerBase
    {

        private readonly string _recaptchaSecret = configuration["Recaptcha:SecretKey"] ?? throw new ArgumentNullException("Not found Recaptcha SecretKey");

        // GET: api/categories/list
        //[Authorize]
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
            // 1. Validate reCAPTCHA Token sent from the MVC Client
            if (string.IsNullOrEmpty(dto.RecaptchaToken))
            {
                return BadRequest(new { message = "reCAPTCHA token is missing." });
            }

            try
            {
                var verifyUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_recaptchaSecret}&response={dto.RecaptchaToken}";
                var googleResponse = await httpClient.PostAsync(verifyUrl, null);

                if (!googleResponse.IsSuccessStatusCode)
                {
                    return StatusCode(500, new { message = "Failed to communicate with Google reCAPTCHA server." });
                }

                var jsonString = await googleResponse.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonString);
                bool isHuman = jsonDoc.RootElement.GetProperty("success").GetBoolean();

                if (!isHuman)
                {
                    return BadRequest(new { message = "reCAPTCHA verification failed. Bot detected!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[reCAPTCHA Error] Verification process failed: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred during reCAPTCHA validation." });
            }

            // 2. Save category via service layer
            var createdDto = await service.CreateAsync(dto);

            // 3. Publish event to RabbitMQ Topic Exchange asynchronously
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"
                };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                // Declare a Topic Exchange instead of a hardcoded queue
                string exchangeName = "shop-online-exchange";
                await channel.ExchangeDeclareAsync(exchange: exchangeName,
                                                   type: ExchangeType.Topic,
                                                   durable: true);

                // Map data from your created DTO to the shared message contract
                var categoryEvent = new CategoryCreatedEvent
                {
                    Id = createdDto.Id,
                    Name = createdDto.Name,
                    CreatedAt = DateTime.UtcNow
                };

                var messageJson = JsonSerializer.Serialize(categoryEvent);
                var body = Encoding.UTF8.GetBytes(messageJson);

                // Publish to the TOPIC EXCHANGE with a structured routing key
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

            // 4. Return the standard 201 Created response
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
