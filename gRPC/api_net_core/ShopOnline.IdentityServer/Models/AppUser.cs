using Microsoft.AspNetCore.Identity;

namespace ShopOnline.IdentityServer.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
