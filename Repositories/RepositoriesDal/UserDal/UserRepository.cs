using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.UserDal;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppointmentDbContext _context;
    public UserRepository(AppointmentDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        await _context.Set<UserRole>().AddAsync(userRole);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
                             .Include(u => u.Roles) // Rolleri de dahil ediyoruz
                             .FirstOrDefaultAsync(u => u.Id == id);
    }

}