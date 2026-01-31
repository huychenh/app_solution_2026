using ShopOnline.Common;

namespace ShopOnline.Api.Integrations.PayPal
{
    public interface IPaymentGateway
    {
        /// <summary>
        /// Creates a PayPal order and returns the approval URL
        /// where the user must be redirected to approve the payment.
        /// </summary>
        Task<PayPalCreateOrderResult> CreateOrderAsync(
            CreatePaymentDto request
        );

        /// <summary>
        /// Captures an approved PayPal order.
        /// This operation must be idempotent.
        /// </summary>
        Task<string> CaptureAsync(string payPalOrderId);
    }
}
