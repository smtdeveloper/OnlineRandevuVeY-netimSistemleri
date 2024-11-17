using Entities.Enums;

namespace Entities.DTOs.UserRole;

public class UserRoleDto
{
    public Guid UserId { get; set; }
    public UserRoles Role { get; set; }
    public bool IsDelete { get; set; } = false;
}