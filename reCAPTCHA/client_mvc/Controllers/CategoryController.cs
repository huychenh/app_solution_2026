using System.Net;
using System.Net.Http.Headers;
using System.Text;
using client_mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopOnline.Common;

namespace client_mvc.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CategoryController(ILogger<CategoryController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: Category
        [HttpGet]
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

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];
                var response = await client.GetAsync($"{apiBase}/api/Categories/list");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // FIXED: Using TempData instead of ViewBag to match Index.cshtml
                    TempData["ErrorMessage"] = "You are not authorized to view this page. Please log in again.";
                    return View("Index", null);
                }

                if (!response.IsSuccessStatusCode)
                {
                    // FIXED: Using TempData instead of ViewBag to match Index.cshtml
                    TempData["ErrorMessage"] = "An error occurred while fetching the Category list from the server.";
                    return View("Index", null);
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var Categorys = JsonConvert.DeserializeObject<List<CategoryReadDto>>(jsonData);

                return View(Categorys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching Categorys.");
                // FIXED: Using TempData instead of ViewBag to match Index.cshtml
                TempData["ErrorMessage"] = "An error occurred while fetching the Category list.";
                return View("Index", null);
            }
        }

        // GET: Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            // 1. EXTRACT & VALIDATE: Safely retrieve the reCAPTCHA token from the form collection
            string recaptchaToken = string.Empty;
            if (Request.HasFormContentType && Request.Form.ContainsKey("g-recaptcha-response"))
            {
                recaptchaToken = Request.Form["g-recaptcha-response"].ToString();
            }

            if (string.IsNullOrEmpty(recaptchaToken))
            {
                // Bind the validation error directly to the RecaptchaToken field for proper UI rendering
                ModelState.AddModelError("RecaptchaToken", "Please complete the reCAPTCHA verification.");
            }

            // 2. ASSIGN: Pass the token to the view model for API processing
            model.RecaptchaToken = recaptchaToken;

            // 3. CHECK MODEL STATE: Stop execution if there are any validation errors (including reCAPTCHA)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 4. AUTHENTICATION: Retrieve the access token required for API calls
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Access token is missing. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                // 5. MAPPING DATA: Populate audit fields and operational metadata
                var currentUserName = User?.Identity?.Name ?? "Unknown Admin";
                model.CreatedBy = currentUserName;
                model.UpdatedBy = currentUserName;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                model.IsActived = true;
                model.IsDeleted = false;

                // 6. CALL API: Initialize the HTTP client and forward the payload to the backend service
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync($"{apiBase}/api/Categories/create", content);

                // 7. HANDLE RESPONSE: Process the result returned from the backend API
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "Authorization failed. You do not have permission to create a category.";
                    return View(model);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var apiErrorResponse = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = !string.IsNullOrEmpty(apiErrorResponse) && apiErrorResponse.Contains("message")
                        ? $"Failed to create Category: {apiErrorResponse}"
                        : $"Failed to create Category. Server responded with status code: {(int)response.StatusCode}";

                    return View(model);
                }

                TempData["SuccessMessage"] = "Great! The new category has been created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Category.");
                TempData["ErrorMessage"] = "An unexpected error occurred while saving the category.";
                return View(model);
            }
        }

        // GET: Category/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
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

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];

                // Call API with the specified Id (e.g., https://localhost:7210/api/Categories/getbyid/5)
                var response = await client.GetAsync($"{apiBase}/api/Categories/getbyid/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // FIXED: Using TempData instead of ViewBag to match Index.cshtml
                    TempData["ErrorMessage"] = "You are not authorized to view this page. Please log in again.";
                    return View("Index", null);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
                    return RedirectToAction("Index");
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "An error occurred while fetching the Category details.";
                    return RedirectToAction("Index");
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryReadDto>(jsonData);

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching details for Category ID: {id}.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }

        // GET: Category/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login", "Account");
                }

                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];

                var response = await client.GetAsync($"{apiBase}/api/Categories/getbyid/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                    return RedirectToAction("Index");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
                    return RedirectToAction("Index");
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Unable to retrieve category information.";
                    return RedirectToAction("Index");
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryReadDto>(jsonData);

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category delete page.");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index");
            }
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Access token is missing. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];

                var response = await client.DeleteAsync(
                    $"{apiBase}/api/Categories/delete/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] =
                        "Authorization failed. You do not have permission to delete this category.";
                    return RedirectToAction("Index");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] =
                        $"Category with ID {id} was not found.";
                    return RedirectToAction("Index");
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] =
                        $"Failed to delete category. Status code: {(int)response.StatusCode}";
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] =
                    "Category deleted successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category ID: {id}");
                TempData["ErrorMessage"] =
                    "An unexpected error occurred while deleting the category.";
                return RedirectToAction("Index");
            }
        }



        // GET: Category/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login", "Account");
                }

                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];
                if (string.IsNullOrEmpty(apiBase))
                {
                    _logger.LogError("API Base URL configuration 'BaseURLSettings:ShopOnline_Api_Url' is missing.");
                    TempData["ErrorMessage"] = "Server configuration error.";
                    return RedirectToAction("Index");
                }

                var response = await client.GetAsync($"{apiBase}/api/Categories/getbyid/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
                    return RedirectToAction("Index");
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Unable to load category information.";
                    return RedirectToAction("Index");
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryReadDto>(jsonData);

                // FIXED: Check if deserialization returned null to prevent NullReferenceException
                if (category == null)
                {
                    _logger.LogError($"Failed to deserialize category data for ID: {id}");
                    TempData["ErrorMessage"] = "Category data format is invalid.";
                    return RedirectToAction("Index");
                }

                var model = new CategoryUpdateViewModel
                {
                    Id = category.Id,
                    // FIXED: Safe null-coalescing fallback for string properties
                    Name = category.Name ?? string.Empty,
                    Description = category.Description ?? string.Empty
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading category edit page for ID: {id}");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index");
            }
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateViewModel model)
        {
            // FIXED: Ensure model is not null before checking ModelState
            if (model == null)
            {
                TempData["ErrorMessage"] = "Invalid request data.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Access token is missing.";
                    return RedirectToAction("Login", "Account");
                }

                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiBase = _configuration["BaseURLSettings:ShopOnline_Api_Url"];
                if (string.IsNullOrEmpty(apiBase))
                {
                    _logger.LogError("API Base URL configuration is missing during postback.");
                    TempData["ErrorMessage"] = "Server configuration error.";
                    return View(model);
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{apiBase}/api/Categories/update/{model.Id}", content);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to update category.";
                    return View(model);
                }

                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with ID: {model.Id}");
                TempData["ErrorMessage"] = "An unexpected error occurred while updating category.";
                return View(model);
            }
        }
    }
}