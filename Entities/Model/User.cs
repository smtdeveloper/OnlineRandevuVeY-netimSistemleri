namespace Entities.Model;

public class User : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsDelete { get; set; } = false;

    public ICollection<UserRole> Roles{ get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
}