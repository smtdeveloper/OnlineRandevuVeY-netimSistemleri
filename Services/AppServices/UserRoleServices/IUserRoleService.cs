using Entities.DTOs.UserRole;
using Entities.Model;
using System.Linq.Expressions;


namespace Services.AppServices.UserRoleServices
{
    public interface IUserRoleService 
    {
        Task<ServiceResult<List<UserRoleDto>>> GetAll();
        Task<ServiceResult<List<UserRoleDto>>> Where(Expression<Func<UserRole, bool>> expression);
        Task<ServiceResult<UserRoleDto?>> GetByIdAsync(Guid id);
        Task<ServiceResult<CreateUserRoleResponse>> CreateAsync(CreateUserRoleRequest request);
        Task<ServiceResult<bool>> SoftDeleteAsync(Guid id);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
        Task<ServiceResult<bool>> UpdateAsync(UpdateUserRoleRequest request);
    }
}