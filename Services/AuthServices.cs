
using e_commerce_web_api.Data;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.EntityFrameworkCore;
namespace e_commerce_web_api.Services;

public class AuthServices : IAuthService
{

    // call data base
    // dependecny injection
    private readonly AppDbContext _appdbcontext;
    // 
    public AuthServices(AppDbContext appDbContext)
    {
        _appdbcontext = appDbContext;
    }


    // registar user services 

    public async Task<UserDto> RegesterUser(RegisterUserDto userData)
    {

        var findUser = await _appdbcontext.Users.FirstOrDefaultAsync(c => c.Email == userData.Email);
        if (findUser != null)
        {
            throw new InvalidOperationException("This email already exits");
        }


        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = userData.FullName.Trim(),
            Email = userData.Email,
            // change this password with haspassword
            Password = userData.Password,
            CreatedAt = DateTime.UtcNow
        };
        // newUser.PasswordHash = _passwordHasher.HashPassword(
        //        newUser,
        //        userData.Password
        //    );


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

}
