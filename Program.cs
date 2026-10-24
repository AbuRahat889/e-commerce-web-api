using System.Threading.RateLimiting;
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
using StackExchange.Redis;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// registar controllers
// builder.Services.AddSingleton()
builder.Services.AddScoped<ICategoryService, CategoriesServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// register redis for connect redis server
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("Redis");

    return ConnectionMultiplexer.Connect(connectionString!);
});
// email SMTP server
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// rate limit 
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            ip,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        var redis = context.HttpContext.RequestServices
            .GetRequiredService<IConnectionMultiplexer>();

        var db = redis.GetDatabase();
        var lockoutKey = $"login:lockout:{ip}";

        // Lockout check, 
        await db.StringSetAsync(
            lockoutKey,
            "1",
            expiry: TimeSpan.FromMinutes(15),
            when: When.NotExists
        );

        context.HttpContext.Response.StatusCode = 429;

        await context.HttpContext.Response.WriteAsJsonAsync(
            cancellationToken
        );
    };
});


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CategoryProfile>();
    cfg.AddProfile<ProductProfile>();
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
app.UseRateLimiter();
app.UseMiddleware<LoginLockoutMiddleware>();


// Console.WriteLine($"Redis Test: {value}");
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


