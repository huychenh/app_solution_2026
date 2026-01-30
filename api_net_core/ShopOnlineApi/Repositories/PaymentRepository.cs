using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Models;

namespace ShopOnline.Api.Repositories
{    
    public class PaymentRepository(AppDbContext dbContext) : IPaymentRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        /// <summary>
        /// Creates and persists a new payment record.
        /// </summary>
        public async Task<Payment> CreateAsync(Payment payment)
        {
            payment.Id = Guid.NewGuid();
            payment.CreatedDate = DateTime.UtcNow;

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            return payment;
        }

        /// <summary>
        /// Retrieves a payment by its internal identifier.
        /// </summary>
        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Retrieves a payment by PayPal Order ID.
        /// Used primarily for webhook processing.
        /// </summary>
        public async Task<Payment?> GetByPayPalOrderIdAsync(string payPalOrderId)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PayPalOrderId == payPalOrderId);
        }

        /// <summary>
        /// Updates the payment status in a controlled manner.
        /// Optionally updates the PayPal Capture ID.
        /// </summary>
        public async Task UpdateStatusAsync(Guid paymentId, PaymentStatus status, string? payPalCaptureId = null)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId) ?? throw new InvalidOperationException("Payment not found.");
            payment.Status = status;
            payment.UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(payPalCaptureId))
            {
                payment.PayPalCaptureId = payPalCaptureId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
