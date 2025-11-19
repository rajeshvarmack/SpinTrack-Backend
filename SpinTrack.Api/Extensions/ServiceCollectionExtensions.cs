using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpinTrack.Api.Configuration;
using SpinTrack.Application;
using SpinTrack.Infrastructure;
using System.Text;

namespace SpinTrack.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring services in the application
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures API versioning
        /// </summary>
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        /// <summary>
        /// Configures Swagger/OpenAPI documentation
        /// </summary>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // JWT Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Enter your JWT token below."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Include XML comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            // Configure Swagger to work with API versioning
            services.ConfigureOptions<ConfigureSwaggerOptions>();

            return services;
        }

        /// <summary>
        /// Configures CORS policy
        /// </summary>
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:4200" };

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS") // Specific methods only
                          .WithHeaders("Authorization", "Content-Type", "Accept", "X-Requested-With") // Specific headers
                          .WithExposedHeaders("api-supported-versions", "api-deprecated-versions", "X-Pagination") // Expose custom headers
                          .SetPreflightMaxAge(TimeSpan.FromMinutes(10)) // Cache preflight requests
                          .AllowCredentials(); // Allow cookies/credentials
                });
            });

            return services;
        }

        /// <summary>
        /// Configures HSTS (HTTP Strict Transport Security) for production
        /// </summary>
        public static IServiceCollection AddHstsConfiguration(this IServiceCollection services, IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                services.AddHsts(options =>
                {
                    options.MaxAge = TimeSpan.FromDays(365);
                    options.IncludeSubDomains = true;
                    options.Preload = true;
                });
            }

            return services;
        }

        /// <summary>
        /// Configures JWT authentication
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        /// <summary>
        /// Configures database context and health checks
        /// </summary>
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add DbContext
            services.AddDbContext<SpinTrackDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<SpinTrackDbContext>();

            return services;
        }
    }
}
