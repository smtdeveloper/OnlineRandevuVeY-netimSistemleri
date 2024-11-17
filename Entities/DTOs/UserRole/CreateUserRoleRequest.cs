using Entities.Enums;

namespace Entities.DTOs.UserRole;

public class CreateUserRoleRequest
{
    public Guid UserId { get; set; }
    public UserRoles Role { get; set; }
}