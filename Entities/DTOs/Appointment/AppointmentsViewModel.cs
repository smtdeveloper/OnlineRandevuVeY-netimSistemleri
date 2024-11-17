namespace Entities.DTOs.Appointment
{
    public class AppointmentsViewModel
    {
        public bool IsCustomer { get; set; }
        public bool IsAdmin { get; set; }
        public List<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    }
}