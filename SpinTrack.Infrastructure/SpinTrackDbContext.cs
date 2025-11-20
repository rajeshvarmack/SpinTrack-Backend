using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Core.Entities.Auth;
using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Entities.Role;

namespace SpinTrack.Infrastructure
{
    /// <summary>
    /// Main database context for SpinTrack application
    /// </summary>
    public class SpinTrackDbContext : DbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public SpinTrackDbContext(DbContextOptions<SpinTrackDbContext> options, ICurrentUserService? currentUserService = null)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Role> Roles => Set<Role>();

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
            var currentUserId = _currentUserService?.UserId ?? Guid.Empty;
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
