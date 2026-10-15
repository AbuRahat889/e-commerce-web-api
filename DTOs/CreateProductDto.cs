using System;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_web_api.DTOs;

public class CreateProductDto
{

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string ProductName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal Price { get; set; } = 0;

    [Required]
    // [NotEqualToEmptyGuid]
    public Guid CategoryId { get; set; }
}
