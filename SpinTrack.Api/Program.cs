using Serilog;
using SpinTrack.Api.Extensions;
using SpinTrack.Application;
using SpinTrack.Infrastructure;

// ============================================
// Configure Serilog (Before WebApplicationBuilder)
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/spintrack-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("==================================================");
    Log.Information("Starting SpinTrack API application");
    Log.Information("Environment: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");
    Log.Information("==================================================");

    // ============================================
    // Create WebApplication Builder
    // ============================================
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // ============================================
    // Configure Services (Dependency Injection)
    // ============================================
    Log.Information("Configuring services...");

    // Core ASP.NET Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpContextAccessor();

    // API Versioning (must be before Swagger)
    builder.Services.AddApiVersioningConfiguration();

    // Swagger/OpenAPI (depends on API versioning)
    builder.Services.AddSwaggerConfiguration();

    // CORS (must be before authentication)
    builder.Services.AddCorsConfiguration(builder.Configuration);

    // HSTS - HTTP Strict Transport Security (Production only)
    builder.Services.AddHstsConfiguration(builder.Environment);

    // Authentication & Authorization (in this order)
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();

    // Database & Health Checks
    builder.Services.AddDatabaseConfiguration(builder.Configuration);

    // Application Layer Services (Business Logic)
    builder.Services.AddApplication();

    // Infrastructure Layer Services (Data Access, External Services)
    builder.Services.AddInfrastructure(builder.Configuration);

    Log.Information("Services configured successfully");

    // ============================================
    // Build Application
    // ============================================
    var app = builder.Build();

    Log.Information("Application built successfully");

    // ============================================
    // Apply Database Migrations
    // ============================================
    await app.MigrateDatabaseAsync();

    // ============================================
    // Configure Middleware Pipeline
    // ============================================
    Log.Information("Configuring middleware pipeline...");
    
    app.ConfigureMiddlewarePipeline();

    Log.Information("Middleware pipeline configured successfully");

    // ============================================
    // Start Application
    // ============================================
    Log.Information("==================================================");
    Log.Information("SpinTrack API application started successfully");
    Log.Information("Swagger UI: {Url}", app.Environment.IsDevelopment() ? "https://localhost:7001" : "Disabled in production");
    Log.Information("Health Check: {Url}/health", app.Environment.IsDevelopment() ? "https://localhost:7001" : "{your-domain}");
    Log.Information("==================================================");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "==================================================");
    Log.Fatal(ex, "Application terminated unexpectedly");
    Log.Fatal(ex, "Error: {Message}", ex.Message);
    if (ex.InnerException != null)
    {
        Log.Fatal(ex.InnerException, "Inner Exception: {Message}", ex.InnerException.Message);
    }
    Log.Fatal(ex, "==================================================");
}
finally
{
    Log.Information("Shutting down SpinTrack API application...");
    Log.CloseAndFlush();
}
