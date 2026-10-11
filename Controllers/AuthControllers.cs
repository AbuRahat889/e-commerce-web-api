using System;
using e_commerce_web_api.Common;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_api.Controllers;


[ApiController]
[Route("/api/v1")]
public class AuthControllers : ControllerBase
{

    private readonly IAuthService _authservece;

    public AuthControllers(IAuthService authService)
    {
        _authservece = authService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userData)
    {
        var result = await _authservece.RegisterUser(userData);

        return Ok(ApiResponse<UserDto>.SuccessResponse(200, "User register successfully!", result));
    }
}

