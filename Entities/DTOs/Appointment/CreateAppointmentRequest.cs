namespace Entities.DTOs.Appointment;
public class CreateAppointmentRequest 
{
    public Guid UserId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
}