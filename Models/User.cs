using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_api.Models;

// use indexing for email 
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName
    {
        get; set;
    } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
