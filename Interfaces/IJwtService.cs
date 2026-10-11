
using e_commerce_web_api.Models;

namespace e_commerce_web_api.Interfaces;

public interface IJwtService
{
    (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(User user);
}
