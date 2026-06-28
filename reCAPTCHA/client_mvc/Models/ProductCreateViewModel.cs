using System.ComponentModel.DataAnnotations;

namespace client_mvc.Models
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.01, 999999999.99, ErrorMessage = "Price must be between 0.01 and 1,000,000,000.")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100.")]
        public int Quantity { get; set; }
    }
}