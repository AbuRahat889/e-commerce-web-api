using System;

namespace e_commerce_web_api.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public string FullName
    {
        get; set;
    } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
