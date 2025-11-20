using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.SubModules.DTOs
{
    public class SubModuleDto
    {
        public Guid SubModuleId { get; set; }
        public Guid ModuleId { get; set; }
        public string SubModuleKey { get; set; } = string.Empty;
        public string SubModuleName { get; set; } = string.Empty;
        public ModuleStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}