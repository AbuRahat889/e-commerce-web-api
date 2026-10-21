using System;
using e_commerce_web_api.DTOs;

namespace e_commerce_web_api.Interfaces;

public interface IAuthService
{
    Task<UserDto> RegisterUser(RegisterUserDto userData);
    Task<AuthResponseDto> SignInUser(LoginDto userdata);
    Task<bool> VerifyEmail(VerifyEmailDto request);
}
