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
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SpinTrackDbContext>();

                Log.Information("Checking for pending database migrations...");

                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Log.Information("Applying {Count} pending migration(s)...", pendingMigrations.Count());
                    await dbContext.Database.MigrateAsync();
                    Log.Information("Database migrations applied successfully");
                }
                else
                {
                    Log.Information("Database is up to date. No pending migrations");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while migrating the database");
                throw;
            }

            return app;
        }
    }
}
