using System;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_web_api.DTOs;

public class RegisterUserDto
{

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
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
