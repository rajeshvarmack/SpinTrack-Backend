using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Modules.DTOs
{
    public class ModuleDto
    {
        public Guid ModuleId { get; set; }
        public string ModuleKey { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public ModuleStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}