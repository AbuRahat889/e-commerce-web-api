using System;
using e_commerce_web_api.DTOs;

namespace e_commerce_web_api.Interfaces;

public interface IAuthService
{
    Task<UserDto> RegesterUser(RegisterUserDto userData);
}
