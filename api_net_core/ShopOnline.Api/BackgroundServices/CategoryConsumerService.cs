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

                // Khởi tạo connection và channel (Không dùng 'using' ở đây)
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

                // 🎯 SỬA ĐỔI: Giới hạn số lượng tin nhắn xử lý đồng thời (Tránh tràn bộ nhớ RAM)
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

                        // Ack tin nhắn thành công
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Catch Block] Error caught: {ex.Message}");

                        // Nack tin nhắn lỗi. Khuyên khích cấu hình thêm DLX trên RabbitMQ Server để không mất dữ liệu
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                // Kích hoạt nhận tin nhắn
                await _channel.BasicConsumeAsync(queue: queueName,
                                                autoAck: false,
                                                consumer: consumer,
                                                cancellationToken: stoppingToken);

                // Giữ service sống
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Critical Error] Could not connect to RabbitMQ: {ex.Message}");
                throw;
            }
        }

        // 🎯 SỬA ĐỔI: Dọn dẹp tài nguyên một cách an toàn khi ứng dụng tắt hẳn
        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}