using ShopOnline.Api.Models;

namespace ShopOnline.Api.Repositories
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// Persists a new payment record when a user initiates a payment.
        /// </summary>
        Task<Payment> CreateAsync(Payment payment);

        /// <summary>
        /// Retrieves a payment by its internal identifier.
        /// Used for querying payment status or administrative purposes.
        /// </summary>
        Task<Payment?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a payment using the PayPal Order ID.
        /// This is essential for handling PayPal webhooks and ensuring idempotency.
        /// </summary>
        Task<Payment?> GetByPayPalOrderIdAsync(string payPalOrderId);

        /// <summary>
        /// Updates the payment status in a controlled and atomic way.
        /// Optionally stores the PayPal Capture ID when the payment is completed.
        /// </summary>
        Task UpdateStatusAsync(Guid paymentId, PaymentStatus status, string? payPalCaptureId = null
        );
    }


}
