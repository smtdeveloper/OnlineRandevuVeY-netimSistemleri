using Entities.Enums;

namespace Entities.Model;

public class Appointment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public bool IsDelete { get; set; } = false;

    public User? User { get; set; } 
    public Service? Service { get; set; }
}
 