using System.ComponentModel.DataAnnotations;

namespace client_mvc.Models
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;        

        public string Description { get; set; } = string.Empty;
    }
}