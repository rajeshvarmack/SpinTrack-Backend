namespace SpinTrack.Application.Features.Permissions.DTOs
{
    public class CreatePermissionRequest
    {
        public Guid SubModuleId { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
    }
}