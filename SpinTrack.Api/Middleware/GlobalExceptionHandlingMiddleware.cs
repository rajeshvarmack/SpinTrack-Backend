using FluentValidation;
using SpinTrack.Application.Common.Results;
using System.Net;
using System.Text.Json;

namespace SpinTrack.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, error) = exception switch
            {
                ValidationException validationEx => (
                    (int)HttpStatusCode.BadRequest,
                    new Error(
                        "VALIDATION_ERROR",
                        "One or more validation errors occurred",
                        validationEx.Errors.GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                    )
                ),
                UnauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                    new Error("UNAUTHORIZED", "Unauthorized access")
                ),
                KeyNotFoundException => (
                    (int)HttpStatusCode.NotFound,
                    new Error("NOT_FOUND", exception.Message)
                ),
                InvalidOperationException => (
                    (int)HttpStatusCode.BadRequest,
                    new Error("INVALID_OPERATION", exception.Message)
                ),
                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    new Error("INTERNAL_SERVER_ERROR", "An unexpected error occurred. Please try again later.")
                )
            };

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(result);
        }
    }
}
