using Entities.Enums;
using System;
using System.Collections.Generic;

namespace Entities.DTOs.Auth
{
    public class AssignRoleRequest
    {
        public Guid UserId { get; set; }
        public UserRoles SelectedRole { get; set; }
        public List<RoleDto> AvailableRoles { get; set; } = new List<RoleDto>();
        public List<RoleDto> AssignedRoles { get; set; } = new List<RoleDto>();
    }

}