using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopOnline.Common.Messages;
using System.Text;
using System.Text.Json;

namespace ShopOnline.Api.BackgroundServices
{
    public class CategoryConsumerService : BackgroundService
    {
        private IConnection? _connection;
        private IChannel? _channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };

                // Initialize connection and channel (Do not use 'using' here to keep them alive)
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                // 1. Declare Topic Exchange
                string exchangeName = "shop-online-exchange";
                await _channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken
                );

                // 2. Declare Queue
                string queueName = "category-created-queue";
                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken
                );

                // 3. Bind Queue
                string routingKey = "category.#";
                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: exchangeName,
                    routingKey: routingKey,
                    cancellationToken: stoppingToken
                );

                Console.WriteLine($"[RabbitMQ] Successfully declared Exchange '{exchangeName}' and bound to Queue '{queueName}'");

                // 🎯 MODIFICATION: Limit the number of unacknowledged messages (Prevents out-of-memory issues)
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var messageJson = Encoding.UTF8.GetString(body);
                        var categoryData = JsonSerializer.Deserialize<CategoryCreatedEvent>(messageJson);

                        if (categoryData?.Name == "Error")
                        {
                            throw new Exception("Simulated processing failure!");
                        }

                        Console.WriteLine($"[Success] Processed: {categoryData?.Name}");

                        // Acknowledge the message successfully processed
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Catch Block] Error caught: {ex.Message}");

                        // Negative acknowledgment for failed messages. Highly recommended to configure DLX on RabbitMQ Server to prevent data loss
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                // Start consuming messages
                await _channel.BasicConsumeAsync(queue: queueName,
                                                autoAck: false,
                                                consumer: consumer,
                                                cancellationToken: stoppingToken);

                // Keep the background service alive
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Critical Error] Could not connect to RabbitMQ: {ex.Message}");
                throw;
            }
        }

        // 🎯 MODIFICATION: Clean up resources safely when the application shuts down
        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}