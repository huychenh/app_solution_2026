using Microsoft.Extensions.Options;
using ShopOnline.Common;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShopOnline.Api.Integrations.PayPal
{
    public class PayPalPaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly PayPalOptions _options;

        public PayPalPaymentGateway(
            HttpClient httpClient,
            IOptions<PayPalOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        /// <summary>
        /// Creates a PayPal order and returns the approval URL.
        /// </summary>
        public async Task<PayPalCreateOrderResult> CreateOrderAsync(
            CreatePaymentDto request)
        {
            var accessToken = await GetAccessTokenAsync();

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                new
                {
                    amount = new
                    {
                        currency_code = request.Currency,
                        value = request.Amount.ToString("F2")
                    }
                }
            },
                application_context = new
                {
                    return_url = "https://localhost:5001/payment/success",
                    cancel_url = "https://localhost:5001/payment/cancel"
                }
            };

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl}/v2/checkout/orders");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var orderId = doc.RootElement.GetProperty("id").GetString()!;
            var approvalUrl = doc.RootElement
                .GetProperty("links")
                .EnumerateArray()
                .First(x => x.GetProperty("rel").GetString() == "approve")
                .GetProperty("href")
                .GetString()!;

            return new PayPalCreateOrderResult
            {
                PayPalOrderId = orderId,
                ApprovalUrl = approvalUrl
            };
        }

        /// <summary>
        /// Captures an approved PayPal order.
        /// </summary>
        public async Task<string> CaptureAsync(string payPalOrderId)
        {
            var accessToken = await GetAccessTokenAsync();

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl}/v2/checkout/orders/{payPalOrderId}/capture");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("purchase_units")[0]
                .GetProperty("payments")
                .GetProperty("captures")[0]
                .GetProperty("id")
                .GetString()!;
        }

        /// <summary>
        /// Retrieves an OAuth2 access token from PayPal.
        /// </summary>
        private async Task<string> GetAccessTokenAsync()
        {
            var authToken = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                    $"{_options.ClientId}:{_options.ClientSecret}"
                )
            );

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl}/v1/oauth2/token");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", authToken);

            request.Content = new StringContent(
                "grant_type=client_credentials",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("access_token")
                .GetString()!;
        }
    }
}
