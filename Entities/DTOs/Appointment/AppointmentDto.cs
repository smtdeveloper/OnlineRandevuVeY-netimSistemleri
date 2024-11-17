using Entities.Enums;

namespace Entities.DTOs.Appointment
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public string UserName { get; set; }       
        public string ServiceName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }                   
        public DateTime CreatedDate { get; set; }
        public bool IsDelete { get; set; }
    }
}