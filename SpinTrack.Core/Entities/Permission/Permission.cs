using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Enums;

namespace SpinTrack.Core.Entities.Permission
{
    public class Permission : BaseEntity
    {
        public Guid PermissionId { get; set; }
        public Guid SubModuleId { get; set; }
        public global::SpinTrack.Core.Entities.SubModule.SubModule? SubModule { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public ModuleStatus Status { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}