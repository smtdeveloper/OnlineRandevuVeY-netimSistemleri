using Entities.DTOs.Appointment;
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

    public async Task<List<AppointmentDto>?> GetAllAppointmentAsync()
    {
        return await _context.Appointments
            .Select(_appointment => new AppointmentDto
            {
                Id = _appointment.Id,
                UserId = _appointment.UserId,
                ServiceId = _appointment.ServiceId,
                ServiceName = _appointment.Service.Name,
                Status = _appointment.Status,
                AppointmentDate = _appointment.AppointmentDate,
                UserName = _appointment.User.UserName,
                CreatedDate = _appointment.CreatedDate,
                IsDelete = _appointment.IsDelete
            })
            .Where(_appointment => _appointment.IsDelete == false)
            .AsNoTracking()            
            .ToListAsync();
    }

    public async Task<List<AppointmentDto>?> GetAllByUserAppointmentAsync(Guid userId)
    {
        return await _context.Appointments
            .Select(_appointment => new AppointmentDto
            {
                Id = _appointment.Id,
                UserId = _appointment.UserId,
                ServiceId = _appointment.ServiceId,
                ServiceName = _appointment.Service.Name,
                Status = _appointment.Status,
                AppointmentDate = _appointment.AppointmentDate,
                UserName = _appointment.User.UserName,
                CreatedDate = _appointment.CreatedDate,
                IsDelete = _appointment.IsDelete
            })
            .Where(_appointment => _appointment.UserId == userId && _appointment.IsDelete == false)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Appointment?> GetByAppointmentIdAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Service) // Service ilişkisinin yüklenmesi
            .Include(u => u.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

}
