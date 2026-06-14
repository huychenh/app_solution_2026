using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;


        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = string.Empty;

        public string UpdatedBy { get; set; } = string.Empty;
        
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
    }
}
