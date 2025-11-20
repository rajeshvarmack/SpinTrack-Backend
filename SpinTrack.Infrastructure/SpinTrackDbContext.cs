using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Core.Entities.Auth;
using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Entities.Role;
using SpinTrack.Core.Entities.Module;
using SpinTrack.Core.Entities.SubModule;
using SpinTrack.Core.Entities.Permission;
using SpinTrack.Core.Entities.Country;
using SpinTrack.Core.Entities.Currency;
using SpinTrack.Core.Entities.TimeZone;
using SpinTrack.Core.Entities.DateFormat;
using SpinTrack.Core.Entities.Company;
using SpinTrack.Core.Entities.BusinessDay;
using SpinTrack.Core.Entities.BusinessHours;
using SpinTrack.Core.Entities.BusinessHoliday;
using SpinTrack.Core.Entities.Product;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Infrastructure
{
    /// <summary>
    /// Main database context for SpinTrack application
    /// </summary>
    public class SpinTrackDbContext : DbContext
    {
        private readonly ICurrentUserService? _currentUser_service;

        public SpinTrackDbContext(DbContextOptions<SpinTrackDbContext> options, ICurrentUserService? currentUserService = null)
            : base(options)
        {
            _currentUser_service = currentUserService;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<SubModule> SubModules => Set<SubModule>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<TimeZoneEntity> TimeZones => Set<TimeZoneEntity>();
        public DbSet<DateFormat> DateFormats => Set<DateFormat>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<BusinessDay> BusinessDays => Set<BusinessDay>();
        public DbSet<BusinessHour> BusinessHours => Set<BusinessHour>();
        public DbSet<BusinessHoliday> BusinessHolidays => Set<BusinessHoliday>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVersion> ProductVersions => Set<ProductVersion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpinTrackDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var currentUserId = _currentUser_service?.UserId ?? Guid.Empty;
            var currentTime = DateTimeOffset.UtcNow;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserId;
                        entry.Entity.CreatedAt = currentTime;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = currentUserId;
                        entry.Entity.ModifiedAt = currentTime;
                        break;
                }
            }
        }
    }
}
