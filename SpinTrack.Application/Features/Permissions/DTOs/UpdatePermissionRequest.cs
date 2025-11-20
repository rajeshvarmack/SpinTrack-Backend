namespace SpinTrack.Application.Features.Permissions.DTOs
{
    public class UpdatePermissionRequest
    {
        public Guid SubModuleId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
    }
}