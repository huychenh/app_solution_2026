using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.IdentityServer.Models;

namespace ShopOnline.IdentityServer.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly string _clientMvcUrl;
        private readonly IConfiguration _configuration;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IIdentityServerInteractionService interaction,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interaction = interaction;
            _configuration = configuration;
            _clientMvcUrl = _configuration["BaseURLSettings:ShopOnline_MvcClient_Url"] ?? throw new InvalidOperationException("ShopOnline_MvcClient_Url configuration is missing.");
        }

        // GET: /auth/login
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /auth/login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);

                    return Redirect("~/");
                }
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        // GET: /auth/logout
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
            return Redirect(logoutRequest?.PostLogoutRedirectUri ?? "/");
        }

        // GET: /account/register
        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpGet]
        public IActionResult Cancel(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect(_clientMvcUrl);
        }


        // POST: /account/register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new AppUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //Assign roles
                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return Redirect("~/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }

}

