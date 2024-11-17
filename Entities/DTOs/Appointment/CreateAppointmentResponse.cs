public class CreateAppointmentResponse
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }

    public CreateAppointmentResponse(Guid id, string serviceName, DateTime appointmentDate, string status)
    {
        Id = id;
        ServiceName = serviceName;
        AppointmentDate = appointmentDate;
        Status = status;
    }

    // Parameterless constructor for serialization/deserialization purposes (optional)
    public CreateAppointmentResponse() { }
}
