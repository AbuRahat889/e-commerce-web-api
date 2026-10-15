using System;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_web_api.Models;

public class Product
{

    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string ProductName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    // Foreign Key
    public Guid CategoryId { get; set; }

    // Navigation Property
    public Category Category { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
