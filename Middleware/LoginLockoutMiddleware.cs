using StackExchange.Redis;
using e_commerce_web_api.Common;

namespace e_commerce_web_api.Middleware;

public class LoginLockoutMiddleware
{
    private readonly RequestDelegate _next;

    public LoginLockoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IConnectionMultiplexer redis)
    {
        var isLoginRequest =
            HttpMethods.IsPost(context.Request.Method) &&
            context.Request.Path.Value?.EndsWith(
                "/login",
                StringComparison.OrdinalIgnoreCase
            ) == true;

        if (!isLoginRequest)
        {
            await _next(context);
            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        var db = redis.GetDatabase();
        var lockoutKey = $"login:lockout:{ip}";

        if (await db.KeyExistsAsync(lockoutKey))
        {
            context.Response.StatusCode =
                StatusCodes.Status429TooManyRequests;

            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.ErrorResponse(
                message: "Too many login attempts.",
                statusCode: 429,
                errors: new List<string>
                {
                    "Your IP is temporarily blocked. Please try again after 15 minutes. from middleware"
                }
            );

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _next(context);
    }
}