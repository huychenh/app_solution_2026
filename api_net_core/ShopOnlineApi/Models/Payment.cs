namespace ShopOnline.Api.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string? ProductId { get; set; }

        public string PayPalOrderId { get; set; } = string.Empty;

        public string? PayPalCaptureId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = string.Empty;

        public PaymentStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
