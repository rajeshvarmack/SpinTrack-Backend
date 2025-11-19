# Extensions Folder

This folder contains extension methods that configure various aspects of the SpinTrack API application.

## Purpose

Extension methods provide a clean, maintainable way to organize startup configuration code. Instead of having a monolithic `Program.cs` file, configuration logic is separated into logical groups.

## Files Overview

### ServiceCollectionExtensions.cs
**Purpose**: Configure services during application startup

**Extension Methods**:
- `AddApiVersioningConfiguration()` - Configures API versioning with URL segment reader
- `AddSwaggerConfiguration()` - Configures Swagger/OpenAPI with JWT security
- `AddCorsConfiguration(IConfiguration)` - Configures CORS policy from appsettings
- `AddHstsConfiguration(IWebHostEnvironment)` - Configures HSTS for production
- `AddJwtAuthentication(IConfiguration)` - Configures JWT Bearer authentication
- `AddDatabaseConfiguration(IConfiguration)` - Configures DbContext and health checks

### WebApplicationExtensions.cs
**Purpose**: Configure the HTTP request pipeline (middleware)

**Extension Methods**:
- `ConfigureMiddlewarePipeline()` - Configures the complete middleware pipeline in the correct order:
  1. Security headers
  2. Global exception handling
  3. Request logging (Serilog)
  4. Swagger UI
  5. HTTPS redirection
  6. HSTS (production only)
  7. CORS
  8. Static files
  9. Authentication & Authorization
  10. Health checks
  11. API controllers

### DatabaseMigrationExtensions.cs
**Purpose**: Handle database migrations and setup

**Extension Methods**:
- `MigrateDatabaseAsync()` - Applies pending Entity Framework migrations with logging

## Usage Example

```csharp
// In Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabaseConfiguration(builder.Configuration);

var app = builder.Build();

// Configure pipeline
await app.MigrateDatabaseAsync();
app.ConfigureMiddlewarePipeline();

app.Run();
```

## Benefits

### 1. Separation of Concerns
Each extension file handles one aspect of configuration:
- **Services** → ServiceCollectionExtensions
- **Middleware** → WebApplicationExtensions
- **Database** → DatabaseMigrationExtensions

### 2. Maintainability
- Easy to find where specific configuration is defined
- Changes are isolated to specific files
- Clear naming conventions

### 3. Testability
- Extension methods can be unit tested
- Dependencies can be mocked
- Configuration logic is isolated

### 4. Reusability
- Extension methods can be used across projects
- Consistent configuration patterns
- Easy to share common configurations

## Adding New Extensions

When adding new configuration:

1. **Choose the right file**:
   - Services configuration → `ServiceCollectionExtensions.cs`
   - Middleware configuration → `WebApplicationExtensions.cs`
   - Database operations → `DatabaseMigrationExtensions.cs`

2. **Follow naming conventions**:
   - Start with `Add` for service configuration
   - Start with `Configure` for middleware configuration
   - Use descriptive names that explain what is being configured

3. **Return the builder**:
   ```csharp
   public static IServiceCollection AddMyFeature(this IServiceCollection services)
   {
       // Configuration code
       return services; // Enable method chaining
   }
   ```

4. **Add XML documentation**:
   ```csharp
   /// <summary>
   /// Configures my new feature with specific settings
   /// </summary>
   public static IServiceCollection AddMyFeature(this IServiceCollection services)
   ```

## Design Patterns Used

### Extension Methods Pattern
- Extends `IServiceCollection` and `WebApplication`
- Provides fluent API for configuration
- Enables method chaining

### Builder Pattern
- Fluent configuration API
- Step-by-step setup
- Clear order of operations

### Dependency Injection
- All configurations use DI
- Services registered in IoC container
- Loose coupling between components

## Configuration Order

### Services Registration Order
The order matters for some services:

1. **Core Services** (Controllers, API Explorer)
2. **API Versioning** (before Swagger)
3. **Swagger/OpenAPI** (after versioning)
4. **CORS** (before authentication)
5. **Authentication** (before authorization)
6. **Authorization** (after authentication)
7. **Database & Health Checks**
8. **Application Services** (domain logic)
9. **Infrastructure Services** (external dependencies)

### Middleware Pipeline Order
The middleware order is critical:

1. **Security Headers** (first, applies to all responses)
2. **Exception Handling** (catch all errors early)
3. **Request Logging** (log all requests)
4. **Swagger UI** (development tool)
5. **HTTPS Redirection** (force HTTPS)
6. **HSTS** (production security)
7. **CORS** (before authentication)
8. **Static Files** (before routing)
9. **Authentication** (identify user)
10. **Authorization** (check permissions)
11. **Endpoints** (route to controllers)

## Best Practices

✅ **DO**:
- Keep extension methods focused on one concern
- Use descriptive method names
- Return the builder for chaining
- Add XML documentation
- Log important configuration steps
- Validate configuration values

❌ **DON'T**:
- Mix service and middleware configuration
- Add business logic to extension methods
- Create dependencies between extension methods
- Hardcode configuration values
- Ignore environment-specific settings

## Environment-Specific Configuration

Some extensions check the environment:

```csharp
// HSTS only in production
if (!environment.IsDevelopment())
{
    services.AddHsts(options => { ... });
}
```

```csharp
// Different Swagger behavior per environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

## Troubleshooting

### Common Issues

**Issue**: Extension method not found
- **Solution**: Check using statement includes `SpinTrack.Api.Extensions`

**Issue**: Configuration not applied
- **Solution**: Verify method is called in Program.cs in correct order

**Issue**: Services not resolved
- **Solution**: Ensure service registration happens before `builder.Build()`

**Issue**: Middleware not executing
- **Solution**: Check middleware order in pipeline

## References

- [ASP.NET Core Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

---

**Maintained by**: SpinTrack Development Team
**Last Updated**: 2025-11-19
