namespace Entities.DTOs.Auth;

public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsDelete { get; set; } = false;
}