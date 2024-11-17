using Entities.Enums;

namespace Entities.DTOs.Appointment;

public class UpdateAppointmentStatusRequest
{
    public Guid Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
