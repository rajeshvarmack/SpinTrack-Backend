using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Enums;

namespace SpinTrack.Core.Entities.Role
{
    /// <summary>
    /// Role entity representing application roles
    /// </summary>
    public class Role : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public RoleStatus Status { get; set; }

        // Soft Delete Properties
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}