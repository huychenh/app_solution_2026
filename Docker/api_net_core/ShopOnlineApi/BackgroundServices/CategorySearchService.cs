using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopOnline.Common.Messages;
using System.Text;
using System.Text.Json;

namespace ShopOnline.Api.BackgroundServices
{
    public class CategorySearchService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"
                };

                // NOTE: Using a 'using' statement here means the connection remains alive as long as this method is running
                using var connection = await factory.CreateConnectionAsync(stoppingToken);
                using var channel = await connection.CreateChannelAsync();

                // 1. 🎯 SYNCHRONIZED: Renamed the exchange to match other microservices
                string exchangeName = "shop-online-exchange";
                await channel.ExchangeDeclareAsync(exchange: exchangeName,
                                                   type: ExchangeType.Topic,
                                                   durable: true,
                                                   cancellationToken: stoppingToken);

                string queueName = "category-search-queue";
                await channel.QueueDeclareAsync(queue: queueName,
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                cancellationToken: stoppingToken);

                // 2. 🎯 OPTIMIZED: Switched to using the '#' wildcard to capture all sub-topics if any exist
                await channel.QueueBindAsync(queue: queueName,
                                             exchange: exchangeName,
                                             routingKey: "category.#",
                                             cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var messageJson = Encoding.UTF8.GetString(body);
                        var categoryData = JsonSerializer.Deserialize<CategoryCreatedEvent>(messageJson);

                        Console.WriteLine($"[Search Service] Syncing to Elasticsearch -> ID: {categoryData.Id}, Name: {categoryData.Name}");

                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Search Service Error] {ex.Message}");
                        // requeue: false is correct here to prevent an infinite processing loop
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

                // Keep the background service alive until the application shuts down
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Search Service Critical] {ex.Message}");
                throw;
            }
        }
    }
}