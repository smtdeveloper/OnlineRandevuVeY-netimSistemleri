using Entities.Enums;

namespace Entities.DTOs.Auth;

public class UserDto
{
    public Guid Id { get; set; }    
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsDelete { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public List<UserRoles> Roles { get; set; } = new List<UserRoles>();

}
