using SpinTrack.Application.Features.Modules.DTOs;
using SpinTrack.Core.Entities.Module;

namespace SpinTrack.Application.Features.Modules.Mappers
{
    public static class ModuleMapper
    {
        public static ModuleDto ToModuleDto(Module module)
        {
            return new ModuleDto
            {
                ModuleId = module.ModuleId,
                ModuleKey = module.ModuleKey,
                ModuleName = module.ModuleName,
                Status = module.Status,
                CreatedAt = module.CreatedAt
            };
        }

        public static ModuleDetailDto ToModuleDetailDto(Module module)
        {
            return new ModuleDetailDto
            {
                ModuleId = module.ModuleId,
                ModuleKey = module.ModuleKey,
                ModuleName = module.ModuleName,
                Status = module.Status,
                CreatedAt = module.CreatedAt,
                ModifiedAt = module.ModifiedAt
            };
        }

        public static Module ToEntity(CreateModuleRequest request)
        {
            return new Module
            {
                ModuleId = Guid.NewGuid(),
                ModuleKey = request.ModuleKey,
                ModuleName = request.ModuleName,
                Status = Core.Enums.ModuleStatus.Active
            };
        }

        public static void UpdateEntity(Module module, UpdateModuleRequest request)
        {
            module.ModuleName = request.ModuleName;
        }
    }
}