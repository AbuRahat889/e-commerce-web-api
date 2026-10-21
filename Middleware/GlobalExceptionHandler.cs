
using e_commerce_web_api.Common;
using e_commerce_web_api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace e_commerce_web_api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred.");

        var (statusCode, message, errors) = exception switch
        {
            EmailAlreadyExistsException ex =>
                (
                    StatusCodes.Status409Conflict,
                    "Email already exists.",
                    new List<string>
                    {
                        ex.Message
                    }
                ),

            InvalidCredentialsException ex =>
                (
                    StatusCodes.Status401Unauthorized,
                    "Invalid email or password.",
                    new List<string>
                    {
                          ex.Message
                    }
                ),
            EmailNotVerifiedException ex =>
                (
                    StatusCodes.Status403Forbidden,
                    "Email verification required.",
                    new List<string>
                    {
                             ex.Message
                    }
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    new List<string>
                    {
                        "Something went wrong. Please try again later."
                    }
                )
        };

        httpContext.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.ErrorResponse(
            message: message,
            statusCode: statusCode,
            errors: errors
        );

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken
        );

        return true;
    }
}
