using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Roles.DTOs
{
    /// <summary>
    /// Role DTO (lightweight)
    /// </summary>
    public class RoleDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public RoleStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}