namespace SpinTrack.Application.Features.SubModules.DTOs
{
    public class CreateSubModuleRequest
    {
        public Guid ModuleId { get; set; }
        public string SubModuleKey { get; set; } = string.Empty;
        public string SubModuleName { get; set; } = string.Empty;
    }
}