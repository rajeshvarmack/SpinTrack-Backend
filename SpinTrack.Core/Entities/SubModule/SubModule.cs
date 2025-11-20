using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Enums;

namespace SpinTrack.Core.Entities.SubModule
{
    /// <summary>
    /// SubModule entity representing child modules
    /// </summary>
    public class SubModule : BaseEntity
    {
        public Guid SubModuleId { get; set; }

        // Foreign key to Module
        public Guid ModuleId { get; set; }
        public Module.Module? Module { get; set; }

        public string SubModuleKey { get; set; } = string.Empty;
        public string SubModuleName { get; set; } = string.Empty;
        public ModuleStatus Status { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}