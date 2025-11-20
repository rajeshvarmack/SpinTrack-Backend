namespace SpinTrack.Application.Features.Roles.DTOs
{
    /// <summary>
    /// Request DTO for creating a role
    /// </summary>
    public class CreateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}