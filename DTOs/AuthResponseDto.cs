
namespace e_commerce_web_api.DTOs;

public class AuthResponseDto
{
    public UserDto User { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
