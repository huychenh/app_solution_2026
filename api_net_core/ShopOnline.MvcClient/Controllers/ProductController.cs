using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopOnline.Common;
using ShopOnline.MvcClient.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ShopOnline.MvcClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProductController(ILogger<ProductController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                // Create an HTTP client to call the API
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"]; //https://localhost:7210                
                var response = await client.GetAsync($"{apiBase}/api/products/list");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.ErrorMessage = "You are not authorized to view this page. Please log in again.";
                    return View("Index", null);
                }

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "An error occurred while fetching the product list.";
                    return View("Index", null);
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductReadDto>>(jsonData);

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                ViewBag.ErrorMessage = "An error occurred while fetching the product list.";
                return View("Index", null);
            }

        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Access token is missing. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{apiBase}/api/products/create", content);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "You are not authorized to create products. (Error Code: " + (int)response.StatusCode + ")");
                    return View(model);
                }

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create product. Please try again. (Error Code: " + (int)response.StatusCode + ")");
                    return View(model);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                return View(model);
            }
        }
    }
}
