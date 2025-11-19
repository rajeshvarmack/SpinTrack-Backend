using Asp.Versioning.ApiExplorer;
using Serilog;
using SpinTrack.Api.Middleware;

namespace SpinTrack.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring the HTTP request pipeline
    /// </summary>
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Configures the middleware pipeline
        /// </summary>
        public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
        {
            // Security Headers Middleware (First for all responses)
            app.UseMiddleware<SecurityHeadersMiddleware>();

            // Global Exception Handling Middleware
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            // Serilog Request Logging
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name ?? "Anonymous");
                    diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                };
            });

            // Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"SpinTrack API {description.GroupName.ToUpperInvariant()}");
                }

                options.RoutePrefix = string.Empty;
                options.DocumentTitle = "SpinTrack API Documentation";
                options.DisplayRequestDuration();
            });

            // HTTPS Redirection
            app.UseHttpsRedirection();

            // HSTS (Production only)
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            // CORS
            app.UseCors("AllowSpecificOrigins");

            // Static Files (for local file storage)
            app.UseStaticFiles();

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Health Check Endpoint
            app.MapHealthChecks("/health");

            // API Controllers
            app.MapControllers();

            return app;
        }
    }
}
