using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SpinTrack.Infrastructure.DesignTime
{
    /// <summary>
    /// Design-time factory to enable EF Core tools (migrations) to create the DbContext
    /// </summary>
    public class SpinTrackDbContextFactory : IDesignTimeDbContextFactory<SpinTrackDbContext>
    {
        public SpinTrackDbContext CreateDbContext(string[] args)
        {
            // Try to locate appsettings.json in API project (assumes usual solution layout)
            var current = Directory.GetCurrentDirectory();
            string? apiPath = null;

            // Ascend directories to find SpinTrack.Api folder
            var dir = new DirectoryInfo(current);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "SpinTrack.Api");
                if (Directory.Exists(candidate))
                {
                    apiPath = candidate;
                    break;
                }
                dir = dir.Parent;
            }

            // Fallback to current directory if not found
            var basePath = apiPath ?? current;

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Could not find a connection string named 'DefaultConnection'. " +
                    $"Make sure appsettings.json exists in '{basePath}' and contains the connection string.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<SpinTrackDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString, 
                b => b.MigrationsAssembly(typeof(SpinTrackDbContext).Assembly.FullName));

            // Pass null for ICurrentUserService (not needed at design-time)
            return new SpinTrackDbContext(optionsBuilder.Options, null);
        }
    }
}
