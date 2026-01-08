using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Email { get; set; }
                
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
                
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
