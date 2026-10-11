
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using e_commerce_web_api.Configurations;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace e_commerce_web_api.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public (string AccessToken, DateTime ExpiresAt)
        GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var expiresAt = DateTime.UtcNow.AddMinutes(
            _jwtSettings.AccessTokenMinutes
        );

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.NameId, user.Id.ToString()
            ),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var token = new JwtSecurityToken(   
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return (accessToken, expiresAt);
    }
}
