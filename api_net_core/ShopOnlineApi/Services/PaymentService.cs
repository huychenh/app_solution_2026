using ShopOnline.Api.Integrations.PayPal;
using ShopOnline.Api.Models;
using ShopOnline.Api.Repositories;
using ShopOnline.Common;
using System.Text.Json;

namespace ShopOnline.Api.Services
{
    public class PaymentService(IPaymentRepository paymentRepository, IPaymentGateway paymentGateway) : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        private readonly IPaymentGateway _paymentGateway = paymentGateway;

        /// <summary>
        /// Initiates a PayPal payment.
        /// Creates an internal payment record and a PayPal order.
        /// </summary>
        public async Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentDto request, string userId)
        {
            // 1. Create internal payment record
            var payment = new Payment
            {
                UserId = userId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = PaymentStatus.Created
            };

            payment = await _paymentRepository.CreateAsync(payment);

            // 2. Create PayPal order
            var payPalResult = await _paymentGateway.CreateOrderAsync(request);

            // 3. Update payment with PayPal order information
            payment.PayPalOrderId = payPalResult.PayPalOrderId;

            await _paymentRepository.UpdateStatusAsync(
                payment.Id,
                PaymentStatus.Pending
            );

            // 4. Return data required by frontend
            return new CreatePaymentResultDto
            {
                PaymentId = payment.Id,
                PayPalOrderId = payPalResult.PayPalOrderId,
                ApprovalUrl = payPalResult.ApprovalUrl
            };
        }

        /// <summary>
        /// Captures a PayPal payment after user approval.
        /// This method is idempotent.
        /// </summary>
        public async Task CapturePaymentAsync(string payPalOrderId)
        {
            var payment = await _paymentRepository
                .GetByPayPalOrderIdAsync(payPalOrderId);

            if (payment == null)
                throw new InvalidOperationException("Payment not found.");

            // Prevent double capture
            if (payment.Status == PaymentStatus.Completed)
                return;

            if (payment.Status != PaymentStatus.Pending &&
                payment.Status != PaymentStatus.Approved)
                throw new InvalidOperationException(
                    $"Invalid payment state: {payment.Status}"
                );

            // Capture payment via PayPal
            var captureId = await _paymentGateway.CaptureAsync(payPalOrderId);

            // Update payment status
            await _paymentRepository.UpdateStatusAsync(
                payment.Id,
                PaymentStatus.Completed,
                captureId
            );
        }

        /// <summary>
        /// Handles PayPal webhook events.
        /// Ensures idempotent and state-safe processing.
        /// </summary>
        public async Task HandlePayPalWebhookAsync(string eventType, JsonElement payload)
        {
            // Extract PayPal order ID from webhook payload
            if (!payload.TryGetProperty("resource", out var resource) ||
                !resource.TryGetProperty("id", out var orderIdElement))
                return;

            var payPalOrderId = orderIdElement.GetString();
            if (string.IsNullOrEmpty(payPalOrderId))
                return;

            var payment = await _paymentRepository
                .GetByPayPalOrderIdAsync(payPalOrderId);

            if (payment == null)
                return;

            switch (eventType)
            {
                case "CHECKOUT.ORDER.APPROVED":
                    if (payment.Status == PaymentStatus.Pending)
                    {
                        await _paymentRepository.UpdateStatusAsync(
                            payment.Id,
                            PaymentStatus.Approved
                        );
                    }
                    break;

                case "PAYMENT.CAPTURE.COMPLETED":
                    if (payment.Status != PaymentStatus.Completed)
                    {
                        var captureId = resource
                            .GetProperty("purchase_units")[0]
                            .GetProperty("payments")
                            .GetProperty("captures")[0]
                            .GetProperty("id")
                            .GetString();

                        await _paymentRepository.UpdateStatusAsync(
                            payment.Id,
                            PaymentStatus.Completed,
                            captureId
                        );
                    }
                    break;

                case "PAYMENT.CAPTURE.DENIED":
                case "PAYMENT.CAPTURE.FAILED":
                    await _paymentRepository.UpdateStatusAsync(
                        payment.Id,
                        PaymentStatus.Failed
                    );
                    break;
            }
        }

        /// <summary>
        /// Retrieves a payment by its identifier.
        /// </summary>
        public async Task<Payment?> GetPaymentAsync(Guid paymentId)
        {
            return await _paymentRepository.GetByIdAsync(paymentId);
        }
    }
}
