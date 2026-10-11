
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

        int statusCode;
        string message;
        List<string> errors;

        if (exception is EmailAlreadyExistsException)
        {
            statusCode = StatusCodes.Status409Conflict;
            message = "Email already exists.";
            errors = new List<string>
            {
                "Please use a different email address."
            };
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "An unexpected error occurred.";
            errors = new List<string>
            {
                "Something went wrong. Please try again later."
            };
        }

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
