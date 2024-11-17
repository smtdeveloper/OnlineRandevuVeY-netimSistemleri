using Entities.Enums;

public class CreateAppointmentResponse
{
    public Guid Id { get; set; }
    public string? ServiceName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }    

}