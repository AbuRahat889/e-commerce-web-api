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


    // registar api 
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto data)
    {
        var result = await _authservece.RegisterUser(data);

        return Ok(ApiResponse<UserDto>.SuccessResponse(200, "User register successfully!", result));
    }

    // login api 
    [HttpPost("login")]
    public async Task<IActionResult> SignInUser([FromBody] LoginDto data)
    {
        Console.WriteLine($"from controller : {data}");

        var result = await
         _authservece.SignInUser(data);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(200, "Sign in successfully!", result));
    }

    // verify otp 

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
    {
        var result = await _authservece.VerifyEmail(request);

        return Ok(new
        {
            success = result,
            message = "Email verified successfully."
        });
    }


}

