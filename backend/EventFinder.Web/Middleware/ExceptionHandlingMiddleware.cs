using System.Net;
using System.Text.Json;

namespace EventFinder.Web.Middleware
{
    /// <summary>
    /// Catches unhandled exceptions on API routes (/api/*) and returns a consistent
    /// JSON error response instead of letting the default HTML error page render.
    /// MVC (non-API) routes are unaffected and keep using UseExceptionHandler("/Home/Error").
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (context.Request.Path.StartsWithSegments("/api"))
            {
                _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var payload = JsonSerializer.Serialize(new
                {
                    status = context.Response.StatusCode,
                    message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError
                        ? "An unexpected error occurred."
                        : ex.Message
                });

                await context.Response.WriteAsync(payload);
            }
        }
    }
}
