using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Enums;

namespace SpinTrack.Core.Entities.Module
{
    /// <summary>
    /// Module entity representing application modules
    /// </summary>
    public class Module : BaseEntity
    {
        public Guid ModuleId { get; set; }
        public string ModuleKey { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public ModuleStatus Status { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}