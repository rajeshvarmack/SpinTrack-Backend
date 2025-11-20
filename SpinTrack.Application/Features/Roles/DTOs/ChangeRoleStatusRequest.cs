using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Roles.DTOs
{
    /// <summary>
    /// Request DTO for changing role status
    /// </summary>
    public class ChangeRoleStatusRequest
    {
        public RoleStatus Status { get; set; }
    }
}