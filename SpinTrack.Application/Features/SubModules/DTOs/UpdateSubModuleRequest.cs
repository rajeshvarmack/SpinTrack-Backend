namespace SpinTrack.Application.Features.SubModules.DTOs
{
    public class UpdateSubModuleRequest
    {
        public Guid ModuleId { get; set; }
        public string SubModuleName { get; set; } = string.Empty;
    }
}