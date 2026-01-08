using System;
using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Common
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
