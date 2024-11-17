using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.AppointmentDal;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    private readonly AppointmentDbContext _context;

    public AppointmentRepository(AppointmentDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByAppointmentIdAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Service) // Service ilişkisinin yüklenmesi
            .Include(u => u.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

}
