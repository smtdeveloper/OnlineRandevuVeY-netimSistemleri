using Entities.Enums;

namespace Entities.DTOs.UserRole;

public class UpdateUserRoleRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserRoles Role { get; set; }
}

public class UpdateAppointmentResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ServiceName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
}

