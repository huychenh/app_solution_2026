using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ShopOnline.Common;

namespace client_mvc.Controllers
{
    //[Authorize]
    public class PayPalController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PayPalController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: /PayPal
        public IActionResult Index()
        {
            return View();
        }

        // POST: /PayPal/CreatePayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment()
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized();
            }

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new CreatePaymentDto
            {
                Amount = 1,
                Currency = "USD",
                ReferenceId = "DEMO_ORDER_001"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/paypal/create";

            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CreatePaymentResultDto>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Redirect(result!.ApprovalUrl);
        }

        // GET: /PayPal/Success
        public IActionResult Success(string token)
        {
            ViewBag.PayPalToken = token;
            return View();
        }

        // GET: /PayPal/Cancel
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
