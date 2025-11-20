using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Services;
using SpinTrack.Application.Common.Settings;
using SpinTrack.Application.Features.Auth.Interfaces;
using SpinTrack.Application.Features.Users.Interfaces;
using SpinTrack.Application.Features.Roles.Interfaces;
using SpinTrack.Application.Features.Modules.Interfaces;
using SpinTrack.Infrastructure.Authentication;
using SpinTrack.Infrastructure.Repositories;
using SpinTrack.Infrastructure.Services;

namespace SpinTrack.Infrastructure
{
    /// <summary>
    /// Dependency injection configuration for Infrastructure layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<SpinTrackDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(SpinTrackDbContext).Assembly.FullName)));

            // JWT Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // File Storage Settings
            services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

            // Repositories (specific repositories using EF Core directly)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();

            // Infrastructure Services (technical implementations only)
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICsvExportService, CsvExportService>();
            
            // Excel Export Service - Choose implementation
            // Option 1: EPPlus (Third-party library with rich features)
            // Option 2: OpenXml (Microsoft's official library, no third-party dependency)
            var useOpenXmlForExcel = configuration.GetValue<bool>("ExportSettings:UseOpenXmlForExcel", false);
            
            if (useOpenXmlForExcel)
            {
                services.AddScoped<IExcelExportService, OpenXmlExcelExportService>();
            }
            else
            {
                services.AddScoped<IExcelExportService, ExcelExportService>(); // EPPlus (default)
            }

            // File Storage Services - Register both implementations
            services.AddScoped<LocalFileStorageService>();
            services.AddScoped<AzureBlobStorageService>();

            // Register the appropriate file storage service based on configuration
            services.AddScoped<IFileStorageService>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var storageProvider = configuration.GetValue<string>("FileStorage:Provider") ?? "Local";

                return storageProvider.ToLower() switch
                {
                    "azureblob" => serviceProvider.GetRequiredService<AzureBlobStorageService>(),
                    "local" => serviceProvider.GetRequiredService<LocalFileStorageService>(),
                    _ => serviceProvider.GetRequiredService<LocalFileStorageService>()
                };
            });

            return services;
        }
    }
}
