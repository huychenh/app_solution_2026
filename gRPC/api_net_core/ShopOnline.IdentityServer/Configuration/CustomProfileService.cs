using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using ShopOnline.IdentityServer.Models;
using System.Security.Claims;

namespace ShopOnline.IdentityServer.Configuration
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;

        public CustomProfileService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user == null) return;

            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Subject, user.Id),
                new(JwtClaimTypes.Name, user.FullName ?? user.UserName ?? ""),
                new(JwtClaimTypes.Email, user.Email ?? "")
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var r in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, r));
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}
