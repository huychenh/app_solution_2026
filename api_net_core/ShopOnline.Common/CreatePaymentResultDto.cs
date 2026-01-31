using System;

namespace ShopOnline.Common
{
    public class CreatePaymentResultDto
    {
        /// <summary>
        /// Internal payment identifier.
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// PayPal order identifier.
        /// </summary>
        public string PayPalOrderId { get; set; } = string.Empty;

        /// <summary>
        /// URL to redirect the user for PayPal approval.
        /// </summary>
        public string ApprovalUrl { get; set; } = string.Empty;
    }
}