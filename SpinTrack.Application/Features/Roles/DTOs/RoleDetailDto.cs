using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Roles.DTOs
{
    /// <summary>
    /// Role detail DTO
    /// </summary>
    public class RoleDetailDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public RoleStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}