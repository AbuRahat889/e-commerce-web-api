
using e_commerce_web_api.Common;
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
        // Log the actual exception for debugging
        _logger.LogError(
            exception,
            "An unexpected error occurred.");

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        var response = ApiResponse<object>.ErrorResponse(
            message: "An unexpected error occurred.",
            statusCode: StatusCodes.Status500InternalServerError,
            errors: new List<string>
            {
                "Something went wrong. Please try again later."
            });

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}
