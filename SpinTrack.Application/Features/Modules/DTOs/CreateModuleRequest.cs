namespace SpinTrack.Application.Features.Modules.DTOs
{
    public class CreateModuleRequest
    {
        public string ModuleKey { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
    }
}