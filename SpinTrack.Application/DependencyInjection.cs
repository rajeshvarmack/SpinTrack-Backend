using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SpinTrack.Application.Features.Auth.Interfaces;
using SpinTrack.Application.Features.Users.Interfaces;
using SpinTrack.Application.Features.Roles.Interfaces;
using SpinTrack.Application.Features.Modules.Interfaces;
using SpinTrack.Application.Features.SubModules.Interfaces;
using SpinTrack.Application.Features.Permissions.Interfaces;
using SpinTrack.Application.Features.Countries.Interfaces;
using SpinTrack.Application.Features.Currencies.Interfaces;
using SpinTrack.Application.Features.TimeZones.Interfaces;
using SpinTrack.Application.Features.DateFormats.Interfaces;
using SpinTrack.Application.Features.Companies.Interfaces;
using SpinTrack.Application.Features.BusinessDays.Interfaces;
using SpinTrack.Application.Features.BusinessHours.Interfaces;
using SpinTrack.Application.Features.BusinessHolidays.Interfaces;
using SpinTrack.Application.Features.Products.Interfaces;
using SpinTrack.Application.Services;
using System.Reflection;

namespace SpinTrack.Application
{
    /// <summary>
    /// Dependency injection configuration for Application layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register Application Services (Business Logic)
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserQueryService, UserQueryService>();
            services.AddScoped<IUserProfileService, UserProfileService>();

            // Role services
            services.AddScoped<IRoleService, RoleService>();

            // Module services
            services.AddScoped<IModuleService, ModuleService>();

            // SubModule services
            services.AddScoped<ISubModuleService, SubModuleService>();

            // Permission services
            services.AddScoped<IPermissionService, PermissionService>();

            // Country services
            services.AddScoped<ICountryService, CountryService>();

            // Currency services
            services.AddScoped<ICurrencyService, CurrencyService>();

            // TimeZone services
            services.AddScoped<ITimeZoneService, TimeZoneService>();

            // DateFormat services
            services.AddScoped<IDateFormatService, DateFormatService>();

            // Company services
            services.AddScoped<ICompanyService, CompanyService>();

            // BusinessDay services
            services.AddScoped<IBusinessDayService, BusinessDayService>();

            // BusinessHours services
            services.AddScoped<IBusinessHoursService, BusinessHoursService>();

            // BusinessHoliday services
            services.AddScoped<IBusinessHolidayService, BusinessHolidayService>();

            // Product services
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
