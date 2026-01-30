using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Models;

namespace ShopOnline.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var tokenEndpoint =
                $"{_configuration["BaseURLSettings:ShopOnline_IdentityServerProvider_Url"]}/connect/token";

            var form = new Dictionary<string, string>
            {
                ["client_id"] = _configuration["IdentityServer:ClientId"],           // shop_online_api_client
                ["client_secret"] = _configuration["IdentityServer:ClientSecret"],   // secret
                ["grant_type"] = "password",
                ["username"] = request.Email,
                ["password"] = request.Password,
                ["scope"] = _configuration["IdentityServer:Scope"]                   // shop_online_api
            };

            using var content = new FormUrlEncodedContent(form);

            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Unauthorized(new { message = error });
            }

            var json = await response.Content.ReadAsStringAsync();
            return Ok(json);
        }

    }
}
