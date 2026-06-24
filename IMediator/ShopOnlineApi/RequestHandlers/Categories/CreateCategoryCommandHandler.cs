using MediatR;
using RabbitMQ.Client;
using ShopOnline.Api.Services;
using ShopOnline.Common.Messages;
using System.Text;
using System.Text.Json;

namespace ShopOnline.Api.RequestHandlers.Categories;

public class CreateCategoryCommandHandler(ICategoryService service) : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Map Command data back to your legacy DTO or Entity to reuse existing service logic
        // (Alternatively, you can inject DbContext directly here later)
        var legacyDto = new ShopOnline.Common.CategoryCreateDto
        {
            Name = request.Name,
            Description = request.Description
        };

        var createdDto = await service.CreateAsync(legacyDto);

        // Step 2: Publish event to RabbitMQ Topic Exchange asynchronously
        try
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string exchangeName = "shop-online-exchange";
            await channel.ExchangeDeclareAsync(exchange: exchangeName,
                                               type: ExchangeType.Topic,
                                               durable: true);

            var categoryEvent = new CategoryCreatedEvent
            {
                Id = createdDto.Id,
                Name = createdDto.Name,
                CreatedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(categoryEvent);
            var body = Encoding.UTF8.GetBytes(messageJson);

            await channel.BasicPublishAsync(exchange: exchangeName,
                                            routingKey: "category.created",
                                            mandatory: true,
                                            body: body);
        }
        catch (Exception ex)
        {
            // Fail-safe: Log RabbitMQ errors to ensure DB transaction isn't blocked if messaging fails
            Console.WriteLine($"[RabbitMQ Error] Failed to publish message: {ex.Message}");
        }

        // Step 3: Return the newly generated ID back to the Mediator pipeline
        return createdDto.Id;
    }
}