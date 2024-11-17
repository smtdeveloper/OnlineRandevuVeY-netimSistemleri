using Entities.Enums;

namespace Entities.DTOs.Auth;

public class CreateUserRequest
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRoles Role { get; set; }
}