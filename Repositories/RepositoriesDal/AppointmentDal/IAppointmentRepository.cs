using Entities.DTOs.Appointment;
using Entities.Model;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.AppointmentDal;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<Appointment?> GetByAppointmentIdAsync(Guid id);    
    Task<List<AppointmentDto>?> GetAllAppointmentAsync();    
    Task<List<AppointmentDto>?> GetAllByUserAppointmentAsync(Guid userId);    
}
