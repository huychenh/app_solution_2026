namespace ShopOnline.Api.Integrations.PayPal
{
    public class PayPalOptions
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string BaseUrl { get; set; } = "https://api-m.sandbox.paypal.com";
    }
}
