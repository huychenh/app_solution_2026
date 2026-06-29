namespace ShopOnline.Common
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;

        public bool IsActived { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string RecaptchaToken { get; set; } = string.Empty;
    }
}
