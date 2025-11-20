using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Permissions.DTOs
{
    public class ChangePermissionStatusRequest
    {
        public ModuleStatus Status { get; set; }
    }
}