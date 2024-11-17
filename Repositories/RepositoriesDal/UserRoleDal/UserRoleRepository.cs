using Entities.Model;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.UserRoleDal;

public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    private readonly AppointmentDbContext _context;
    public UserRoleRepository(AppointmentDbContext context) : base(context)
    {
        _context = context;
    }
}