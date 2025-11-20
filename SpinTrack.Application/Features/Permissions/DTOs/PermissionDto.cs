using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Permissions.DTOs
{
    public class PermissionDto
    {
        public Guid PermissionId { get; set; }
        public Guid SubModuleId { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public ModuleStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}