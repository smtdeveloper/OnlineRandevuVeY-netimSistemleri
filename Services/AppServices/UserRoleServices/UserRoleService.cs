using AutoMapper;
using Entities.DTOs.UserRole;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.UserRoleDal;
using Repositories.UnitOfWorks;
using System.Linq.Expressions;

namespace Services.AppServices.UserRoleServices
{
    internal class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserRoleService(IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<CreateUserRoleResponse>> CreateAsync(CreateUserRoleRequest request)
        {
            var userRole = _mapper.Map<UserRole>(request);
            userRole.CreatedDate = DateTime.UtcNow;

            await _userRoleRepository.AddAsync(userRole);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateUserRoleResponse { Id = userRole.Id };
            return new ServiceResult<CreateUserRoleResponse>().Success(response);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(id);
            if (userRole == null)
            {
                return new ServiceResult<bool>().Fail("UserRole not found");
            }

            _userRoleRepository.Delete(userRole);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<List<UserRoleDto>>> GetAll()
        {
            var userRoles = await _userRoleRepository.GetAll().ToListAsync();
            var userRoleDtos = _mapper.Map<List<UserRoleDto>>(userRoles);

            return new ServiceResult<List<UserRoleDto>>().Success(userRoleDtos);
        }

        public async Task<ServiceResult<UserRoleDto?>> GetByIdAsync(Guid id)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(id);
            if (userRole == null)
            {
                return new ServiceResult<UserRoleDto?>().Fail("UserRole not found");
            }

            var userRoleDto = _mapper.Map<UserRoleDto>(userRole);
            return new ServiceResult<UserRoleDto?>().Success(userRoleDto);
        }

        public async Task<ServiceResult<bool>> SoftDeleteAsync(Guid id)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(id);
            if (userRole == null)
            {
                return new ServiceResult<bool>().Fail("UserRole not found");
            }

            userRole.IsDelete = true;
            userRole.DeletedDate = DateTime.UtcNow;
            _userRoleRepository.Update(userRole);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(UpdateUserRoleRequest request)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(request.Id);
            if (userRole == null)
            {
                return new ServiceResult<bool>().Fail("UserRole not found");
            }

            userRole.Role = request.Role;
            _userRoleRepository.Update(userRole);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<List<UserRoleDto>>> Where(Expression<Func<UserRole, bool>> expression)
        {
            var userRoles = await _userRoleRepository.Where(expression).ToListAsync();
            var userRoleDtos = _mapper.Map<List<UserRoleDto>>(userRoles);

            return new ServiceResult<List<UserRoleDto>>().Success(userRoleDtos);
        }
    }
}