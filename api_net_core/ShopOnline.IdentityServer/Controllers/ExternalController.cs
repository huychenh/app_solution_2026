using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.IdentityServer.Models;
using System.Security.Claims;

namespace ShopOnline.IdentityServer.Controllers
{
    [Route("[controller]/[action]")]
    public class ExternalController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        
        public ExternalController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/External/Callback"
            };

            props.Items["scheme"] = scheme;
            props.Items["returnUrl"] = returnUrl;

            return Challenge(props, scheme);
        }


        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var externalAuthResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if (externalAuthResult?.Principal == null)
                throw new Exception("External authentication error");

            var externalUser = externalAuthResult.Principal;
            var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Email claim not found from external provider.");
            }

            // Find or Create user in Identity database
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var resultCreate = await _userManager.CreateAsync(user);
                if (!resultCreate.Succeeded)
                    throw new Exception("Cannot create user from external provider");
            }

            // Sign-in via Identity
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Clear temporary external cookie
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            externalAuthResult.Properties.Items.TryGetValue("returnUrl", out var returnUrl);

            return Redirect(returnUrl ?? "~/");
        }


    }

}
