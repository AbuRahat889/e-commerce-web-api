using System;

namespace e_commerce_web_api.DTOs;

public class ReadProductDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
