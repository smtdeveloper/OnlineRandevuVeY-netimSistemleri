using Entities.Enums;

namespace Entities.Model;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public UserRoles Role { get; set; }
    public bool IsDelete { get; set; } = false;

    public User? User { get; set; }
}
