namespace SpinTrack.Api.Middleware
{
    /// <summary>
    /// Middleware to add security headers to all responses
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // X-Content-Type-Options: Prevents MIME type sniffing
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // X-Frame-Options: Prevents clickjacking attacks
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // X-XSS-Protection: Enables XSS filter in older browsers
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer-Policy: Controls referrer information
            context.Response.Headers["Referrer-Policy"] = "no-referrer";

            // Content-Security-Policy: Prevents XSS and injection attacks
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none';";

            // Permissions-Policy: Controls browser features
            context.Response.Headers["Permissions-Policy"] =
                "accelerometer=(), " +
                "camera=(), " +
                "geolocation=(), " +
                "gyroscope=(), " +
                "magnetometer=(), " +
                "microphone=(), " +
                "payment=(), " +
                "usb=()";

            await _next(context);
        }
    }
}
