using Entities.Enums;

namespace Entities.DTOs.Appointment
{
    public class UpdateAppointmentRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; } 
    }
}