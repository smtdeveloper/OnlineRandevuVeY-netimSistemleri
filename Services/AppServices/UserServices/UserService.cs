using AutoMapper;
using Entities.DTOs.Auth;
using Entities.Enums;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.UserDal;
using Repositories.UnitOfWorks;
using Services.BusinessRules;
using System.Linq.Expressions;
using System.Net;

namespace Services.AppServices.UserServices
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CreateUserResponse>> CreateAsync(CreateUserRequest request)
        {
           
            var user = _mapper.Map<User>(request);
            user.CreatedDate = DateTime.UtcNow;

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = user.Id,
                Role = request.Role,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddUserRoleAsync(userRole);
            await _unitOfWork.SaveChangesAsync();

            var result = new ServiceResult<CreateUserResponse>().Success(new CreateUserResponse(user.Id));
            return result;
        }


        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new ServiceResult<bool>().NotFound("Servis bulunamadı.");
            
            _userRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<List<UserDto>>> GetAll()
        {
            var users = await _userRepository
                .GetAll().Where(u => u.IsDelete == false)
                .Include(u => u.Roles)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);
            return new ServiceResult<List<UserDto>>().Success(userDtos);
        }


        public async Task<ServiceResult<UserDto?>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new ServiceResult<UserDto>().NotFound("Servis bulunamadı.");

            var userDto = _mapper.Map<UserDto>(user);
            return new ServiceResult<UserDto?>().Success(userDto);
        }


        public async Task<ServiceResult<bool>> SoftDeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new ServiceResult<bool>().NotFound("Servis bulunamadı.");

           

            user.IsDelete = true;
            user.DeletedDate = DateTime.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
                return new ServiceResult<bool>().NotFound("Servis bulunamadı.");
            
            user.UserName = request.UserName;                       
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<List<UserDto>>> Where(Expression<Func<User, bool>> expression)
        {
            var users = await _userRepository.Where(expression).ToListAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);
            return new ServiceResult<List<UserDto>>().Success(userDtos);
        }

        public async Task<UserDto?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository
                .Where(u => u.UserName == username && !u.IsDelete)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            var x = _mapper.Map<UserDto>(user);
            return x;
        }

        public async Task<ServiceResult<bool>> AssignRoleAsync(Guid userId, UserRoles role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResult<bool>().Fail("Kullanıcı bulunamadı.");
            }

            if (user.Roles.All(r => r.Role != role))
            {
                user.Roles.Add(new UserRole
                {
                    UserId = userId,
                    Role = role
                });

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return new ServiceResult<bool>().Success(true);
        }


        public async Task<ServiceResult<bool>> RemoveRoleAsync(Guid userId, UserRoles role)
        {
            // Hata ve başarı sonuçlarını döndürebilmek için bir ServiceResult örneği oluşturuyoruz.
            var serviceResult = new ServiceResult<bool>();

            // Kullanıcıyı veritabanından alıyoruz.
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                // Kullanıcı bulunamazsa hata sonucu döndürülür.
                return serviceResult.Fail("Kullanıcı bulunamadı.", HttpStatusCode.NotFound);
            }

            // Kullanıcının belirtilen role sahip olup olmadığını kontrol ediyoruz.
            var userRole = user.Roles.FirstOrDefault(r => r.Role == role);
            if (userRole == null)
            {
                // Kullanıcıda rol yoksa hata sonucu döndürülür.
                return serviceResult.Fail("Bu rol kullanıcıda mevcut değil.", HttpStatusCode.BadRequest);
            }

            // Rol kullanıcıdan kaldırılır.
            user.Roles.Remove(userRole);
            await _unitOfWork.SaveChangesAsync();

            // Başarıyla kaldırıldığında başarılı sonucu döndürür.
            return serviceResult.Success(true);
        }




    }
}
