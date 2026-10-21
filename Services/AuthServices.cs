
using System.Security.Cryptography;
using e_commerce_web_api.Data;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Exceptions;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
namespace e_commerce_web_api.Services;

public class AuthServices : IAuthService
{

    // call data base
    // dependecny injection
    private readonly IDatabase _redisDb;
    private readonly AppDbContext _appdbcontext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    // 
    public AuthServices(AppDbContext appDbContext, IPasswordHasher<User> passwordhasher, IJwtService jwtService, IConnectionMultiplexer redis, IEmailService emailService)
    {
        _appdbcontext = appDbContext;
        _passwordHasher = passwordhasher;
        _jwtService = jwtService;
        _redisDb = redis.GetDatabase();
        _emailService = emailService;
    }

    // generate otp
    private static string GenerateOtp()
    {
        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return otp.ToString("D6");
    }
    // store opt into redis
    private async Task StoreEmailVerificationOtp(
    string email,
    string otp)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var key = $"otp:email-verification:{normalizedEmail}";

        await _redisDb.StringSetAsync(
            key,
            otp,
            TimeSpan.FromMinutes(5));
    }

    // registar user services 
    public async Task<UserDto> RegisterUser(RegisterUserDto userData)
    {

        var findUser = await _appdbcontext.Users.FirstOrDefaultAsync(c => c.Email == userData.Email.Trim());
        if (findUser != null)
        {
            throw new EmailAlreadyExistsException();
        }
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = userData.FullName.Trim(),
            Email = userData.Email,
            Role = userData.Role,
            CreatedAt = DateTime.UtcNow
        };
        newUser.Password = _passwordHasher.HashPassword(
               newUser,
               userData.Password
           );
        _appdbcontext.Users.Add(newUser);
        await _appdbcontext.SaveChangesAsync();

        // otp 
        var otp = GenerateOtp();

        await StoreEmailVerificationOtp(newUser.Email, otp);

        await _emailService.SendEmailAsync(
            newUser.Email,
            "Verify Your Email",
            $"Your email verification OTP is: {otp}\n\nThis OTP expires in 5 minutes.");


        return new UserDto
        {
            Id = newUser.Id,
            FullName = newUser.FullName,
            Email = newUser.Email,
            IsEmailVerified = false,
            CreatedAt = newUser.CreatedAt
        };

    }

    // login user 
    public async Task<AuthResponseDto> SignInUser(LoginDto userdata)
    {

        var findUser = await _appdbcontext.Users.FirstOrDefaultAsync(c => c.Email == userdata.Email.Trim());

        if (findUser == null)
        {
            throw new InvalidCredentialsException();
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            findUser,
            findUser.Password,
            userdata.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }

        var (accessToken, expiresAt) =
    _jwtService.GenerateAccessToken(findUser);

        return new AuthResponseDto
        {
            User = new UserDto
            {
                Id = findUser.Id,
                FullName = findUser.FullName,
                Email = findUser.Email,
                CreatedAt = findUser.CreatedAt,
                Role = findUser.Role
            },
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };
    }

}
