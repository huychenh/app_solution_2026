using System.ComponentModel.DataAnnotations;

namespace ShopOnline.IdentityServer.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string ReturnUrl { get; set; } = "/";
    }

}
