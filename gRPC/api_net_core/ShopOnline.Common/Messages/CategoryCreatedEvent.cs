using System;

namespace ShopOnline.Common.Messages
{
    // The message data that will be sent through RabbitMQ
    public class CategoryCreatedEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
