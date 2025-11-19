using Serilog;
using SpinTrack.Api.Extensions;
using SpinTrack.Application;
using SpinTrack.Infrastructure;

// ============================================
// Configure Serilog
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/spintrack-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting SpinTrack API application");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog();

    // ============================================
    // Configure Services
    // ============================================
    
    // Core Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpContextAccessor();

    // API Versioning
    builder.Services.AddApiVersioningConfiguration();

    // Swagger/OpenAPI
    builder.Services.AddSwaggerConfiguration();

    // CORS
    builder.Services.AddCorsConfiguration(builder.Configuration);

    // HSTS (Production only)
    builder.Services.AddHstsConfiguration(builder.Environment);

    // JWT Authentication
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();

    // Database & Health Checks
    builder.Services.AddDatabaseConfiguration(builder.Configuration);

    // Application & Infrastructure Services
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ============================================
    // Build Application
    // ============================================
    var app = builder.Build();

    // Apply Database Migrations
    await app.MigrateDatabaseAsync();

    // Configure Middleware Pipeline
    app.ConfigureMiddlewarePipeline();

    Log.Information("SpinTrack API application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
