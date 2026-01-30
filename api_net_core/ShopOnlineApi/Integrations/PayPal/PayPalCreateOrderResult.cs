namespace ShopOnline.Api.Integrations.PayPal
{
    public class PayPalCreateOrderResult
    {
        /// <summary>
        /// PayPal-generated order identifier.
        /// </summary>
        public string PayPalOrderId { get; set; } = null!;

        /// <summary>
        /// URL used to redirect the user to PayPal for approval.
        /// </summary>
        public string ApprovalUrl { get; set; } = null!;
    }
}
