using SpinTrack.Application.Features.Roles.DTOs;
using SpinTrack.Core.Entities.Role;

namespace SpinTrack.Application.Features.Roles.Mappers
{
    public static class RoleMapper
    {
        public static RoleDto ToRoleDto(Role role)
        {
            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                Status = role.Status,
                CreatedAt = role.CreatedAt
            };
        }

        public static RoleDetailDto ToRoleDetailDto(Role role)
        {
            return new RoleDetailDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                Status = role.Status,
                CreatedAt = role.CreatedAt,
                ModifiedAt = role.ModifiedAt
            };
        }

        public static Role ToEntity(CreateRoleRequest request)
        {
            return new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = request.RoleName,
                Description = request.Description,
                Status = Core.Enums.RoleStatus.Active
            };
        }

        public static void UpdateEntity(Role role, UpdateRoleRequest request)
        {
            role.Description = request.Description;
        }
    }
}