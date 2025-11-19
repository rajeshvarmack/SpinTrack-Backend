using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Users.DTOs
{
    /// <summary>
    /// Request DTO for changing user status
    /// </summary>
    public class ChangeUserStatusRequest
    {
        public UserStatus Status { get; set; }
    }
}
