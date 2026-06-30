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
                var factory = new ConnectionFactory() { HostName = "localhost" };
                // Lưu ý: Nếu dùng using ở đây, kết nối chỉ sống khi hàm này chưa chạy xong
                using var connection = await factory.CreateConnectionAsync(stoppingToken);
                using var channel = await connection.CreateChannelAsync();

                // 1. 🎯 ĐÃ ĐỒNG BỘ: Đổi tên exchange cho khớp với các service khác
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

                // 2. 🎯 ĐÃ TỐI ƯU: Chuyển sang dùng dấu # để nhận được tất cả sub-topic nếu có
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
                        // requeue: false là chính xác để tránh loop vô hạn
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

                // Giữ service sống cho đến khi ứng dụng tắt
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