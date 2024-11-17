using Entities.Model;
using Repositories.RepositoriesDal.GenericDal;

namespace Repositories.RepositoriesDal.UserDal;

public interface IUserRepository : IGenericRepository<User>
{
    Task AddUserRoleAsync(UserRole userRole);
    Task<User?> GetByIdAsync(Guid id);
}