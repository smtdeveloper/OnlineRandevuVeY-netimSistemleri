using Entities.Model;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.AppointmentDal;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    new Task<Appointment?> GetByAppointmentIdAsync(Guid id); 
    Task<Service?> GetServiceByIdAsync(Guid serviceId); 
}
