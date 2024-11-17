using Entities.DTOs.Appointment;
using Entities.DTOs.UserRole;
using Entities.Model;
using System.Linq.Expressions;

namespace Services.AppServices.AppointmentServices;

public interface IAppointmentService 
{
    Task<ServiceResult<List<AppointmentDto>>> GetAll();
    Task<ServiceResult<List<AppointmentDto>>> Where(Expression<Func<Appointment, bool>> expression);
    Task<ServiceResult<AppointmentDto?>> GetByIdAsync(Guid id);
    Task<ServiceResult<CreateAppointmentResponse>> CreateAsync(CreateAppointmentRequest request);
    Task<ServiceResult<bool>> SoftDeleteAsync(Guid id);
    Task<ServiceResult<bool>> DeleteAsync(Guid id);
    Task<ServiceResult<UpdateAppointmentResponse>> UpdateAsync(UpdateAppointmentRequest request);
    Task<ServiceResult<UpdateAppointmentStatusResponse>> UpdateStatusAsync(UpdateAppointmentStatusRequest request);
    Task<ServiceResult<AppointmentsViewModel>> GetAppointmentsForUserAsync();
}