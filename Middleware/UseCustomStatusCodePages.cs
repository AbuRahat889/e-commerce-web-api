using e_commerce_web_api.Common;

namespace e_commerce_web_api.Middleware;

public static class StatusCodePagesMiddleware
{
    public static IApplicationBuilder UseCustomStatusCodePages(
        this IApplicationBuilder app)
    {
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;

            var message = response.StatusCode switch
            {
                404 => "Resource not found.",
                400 => "Bad request.",
                401 => "Unauthorized.",
                403 => "Forbidden.",
                _ => "Request failed."
            };

            var result = ApiResponse<object>.ErrorResponse(
                message,
                response.StatusCode,
                new List<string> { message }
            );

            await response.WriteAsJsonAsync(result);
        });

        return app;
    }
}
