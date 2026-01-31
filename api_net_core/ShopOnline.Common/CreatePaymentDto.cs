namespace ShopOnline.Common
{
    public class CreatePaymentDto
    {
        /// <summary>
        /// The amount to be paid.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The currency code (e.g. USD, EUR).
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Optional product or order reference.
        /// </summary>
        public string? ReferenceId { get; set; }
    }
}
