using Entities.DTOs.Auth;
using Entities.Enums;
using Entities.Model;
using System.Linq.Expressions;


namespace Services.AppServices.UserServices
{
    public interface IUserService 
    {
        Task<ServiceResult<List<UserDto>>> GetAll();
        Task<ServiceResult<List<UserDto>>> Where(Expression<Func<User, bool>> expression);
        Task<ServiceResult<UserDto?>> GetByIdAsync(Guid id);
        Task<ServiceResult<CreateUserResponse>> CreateAsync(CreateUserRequest request);
        Task<ServiceResult<bool>> SoftDeleteAsync(Guid id);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
        Task<ServiceResult<bool>> UpdateAsync(UpdateUserRequest request);
        Task<UserDto?> AuthenticateUserAsync(string username, string password);
        Task<ServiceResult<bool>> AssignRoleAsync(Guid userId, UserRoles role);
        Task<ServiceResult<bool>> RemoveRoleAsync(Guid userId, UserRoles role);
    }
}
