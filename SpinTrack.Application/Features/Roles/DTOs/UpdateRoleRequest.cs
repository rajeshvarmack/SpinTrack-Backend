namespace SpinTrack.Application.Features.Roles.DTOs
{
    /// <summary>
    /// Request DTO for updating a role
    /// </summary>
    public class UpdateRoleRequest
    {
        public string Description { get; set; } = string.Empty;
    }
}