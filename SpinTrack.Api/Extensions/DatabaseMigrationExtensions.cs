using Microsoft.EntityFrameworkCore;
using Serilog;
using SpinTrack.Infrastructure;

namespace SpinTrack.Api.Extensions
{
    /// <summary>
    /// Extension methods for database migration operations
    /// </summary>
    public static class DatabaseMigrationExtensions
    {
        /// <summary>
        /// Applies any pending database migrations
        /// </summary>
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            try
            {
                Log.Information("=== Database Migration Check Started ===");
                
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SpinTrackDbContext>();

                Log.Information("Checking for pending database migrations...");

                // Check if database can be connected
                var canConnect = await dbContext.Database.CanConnectAsync();
                Log.Information("Database connection status: {CanConnect}", canConnect);

                if (!canConnect)
                {
                    Log.Warning("Cannot connect to database. Check your connection string.");
                    return app;
                }

                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                var pendingMigrationsList = pendingMigrations.ToList();
                
                if (pendingMigrationsList.Any())
                {
                    Log.Information("Found {Count} pending migration(s):", pendingMigrationsList.Count);
                    foreach (var migration in pendingMigrationsList)
                    {
                        Log.Information("  - {Migration}", migration);
                    }
                    
                    Log.Information("Applying migrations...");
                    await dbContext.Database.MigrateAsync();
                    Log.Information("Database migrations applied successfully");
                }
                else
                {
                    Log.Information("Database is up to date. No pending migrations");
                }

                // Log applied migrations
                var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
                Log.Information("Total applied migrations: {Count}", appliedMigrations.Count());
                
                Log.Information("=== Database Migration Check Completed ===");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while migrating the database");
                Log.Error("Error details: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.Error("Inner exception: {InnerMessage}", ex.InnerException.Message);
                }
                throw;
            }

            return app;
        }
    }
}
