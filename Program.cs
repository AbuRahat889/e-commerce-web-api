using e_commerce_web_api.Common;
using e_commerce_web_api.Configurations;
using e_commerce_web_api.Data;
using e_commerce_web_api.Extensions;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Middleware;
using e_commerce_web_api.Models;
using e_commerce_web_api.Profiles;
using e_commerce_web_api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// registar controllers
// builder.Services.AddSingleton()
builder.Services.AddScoped<ICategoryService, CategoriesServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CategoryProfile>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Where(e => e.Value?.Errors.Count > 0)
               .SelectMany(e => e.Value?.Errors != null ? e.Value.Errors.Select(x => x.ErrorMessage) : new List<string>()).ToList();
        return new BadRequestObjectResult(ApiResponse<object>.ErrorResponse("Validation Error", 400, errors));
    };
});

builder.Services.AddEndpointsApiExplorer();

// Bind Jwt settings
// Register JWT service
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

// Before builder.Build()
builder.Services.AddJwtAuthentication(
    builder.Configuration
);


var app = builder.Build();
// middlewares
app.UseExceptionHandler();
app.UseCustomStatusCodePages();

// when configur the https then need to add this 
// app.UseHttpsRedirection();

app.MapGet("/", () => "Welcome to my dotnet application!");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();


