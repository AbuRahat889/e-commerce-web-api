using System.ComponentModel.DataAnnotations;

namespace e_commerce_web_api.DTOs
{
    public class CreateCategoryDto
    {
        // annotation for data validation 
        [Required(ErrorMessage = "Category name is required.")]
        [MinLength(10)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}


