using Entities.Model;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.ServiceDal;

public class ServiceRepository : GenericRepository<Service>, IServiceRepository
{
    private readonly AppointmentDbContext _context;
    public ServiceRepository(AppointmentDbContext context) : base(context)
    {
        _context = context;
    }

}