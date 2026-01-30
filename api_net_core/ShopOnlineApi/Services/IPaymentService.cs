using ShopOnline.Api.Models;
using ShopOnline.Common;
using System.Text.Json;

namespace ShopOnline.Api.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Initiates a PayPal payment and creates a local payment record.
        /// Returns the PayPal approval URL for user redirection.
        /// </summary>
        Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentDto request, string userId);

        /// <summary>
        /// Captures a PayPal payment after the user has approved it.
        /// </summary>
        Task CapturePaymentAsync(string payPalOrderId);

        /// <summary>
        /// Handles PayPal webhook events and updates payment state accordingly.
        /// Must be idempotent.
        /// </summary>
        Task HandlePayPalWebhookAsync(string eventType, JsonElement payload);

        /// <summary>
        /// Retrieves the current status of a payment.
        /// </summary>
        Task<Payment?> GetPaymentAsync(Guid paymentId);
    }

}
