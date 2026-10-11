
using e_commerce_web_api.Data;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Exceptions;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace e_commerce_web_api.Services;

public class AuthServices : IAuthService
{

    // call data base
    // dependecny injection
    private readonly AppDbContext _appdbcontext;
    private readonly IPasswordHasher<User> _passwordHasher;

    // 
    public AuthServices(AppDbContext appDbContext, IPasswordHasher<User> passwordhasher)
    {
        _appdbcontext = appDbContext;
        _passwordHasher = passwordhasher;
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
            CreatedAt = DateTime.UtcNow
        };
        newUser.Password = _passwordHasher.HashPassword(
               newUser,
               userData.Password
           );
        _appdbcontext.Users.Add(newUser);
        await _appdbcontext.SaveChangesAsync();

        return new UserDto
        {
            Id = newUser.Id,
            FullName = newUser.FullName,
            Email = newUser.Email,
            CreatedAt = newUser.CreatedAt
        };

    }

    // login user 
    public async Task<UserDto> SignInUser(LoginDto userdata)
    {

        var findUser = await _appdbcontext.Users.FirstOrDefaultAsync(c => c.Email == userdata.Email);
        Console.WriteLine($"find user ; {findUser}");

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

        return new UserDto
        {
            Id = findUser.Id,
            FullName = findUser.FullName,
            Email = findUser.Email,
            CreatedAt = findUser.CreatedAt
        };
    }

}
