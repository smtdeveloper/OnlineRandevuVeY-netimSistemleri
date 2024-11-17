namespace Entities.Model;

public class Service : BaseEntity
{   
    public string Name { get; set; }
    public bool IsDelete { get; set; } = false;

    public ICollection<Appointment> Appointments { get; set; } = [];
}