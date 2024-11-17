using AutoMapper;
using Entities.DTOs.Appointment;
using Entities.DTOs.UserRole;
using Entities.Enums;
using Entities.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.AppointmentDal;
using Repositories.RepositoriesDal.ServiceDal;
using Repositories.UnitOfWorks;
using Services.Constansts;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Services.AppServices.AppointmentServices;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AppointmentService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork, IMapper mapper, IServiceRepository serviceRepository, IHttpContextAccessor  httpContextAccessor )
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResult<CreateAppointmentResponse>> CreateAsync(CreateAppointmentRequest request)
    {
        if (request.UserId == Guid.Empty)
                return new ServiceResult<CreateAppointmentResponse>().Fail(Messages.UserNotFound);

            Appointment appointment = _mapper.Map<Appointment>(request);
            appointment.Status = AppointmentStatus.Pending;
            appointment.CreatedDate = DateTime.UtcNow;

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                return new ServiceResult<CreateAppointmentResponse>().Fail("Service not found.");
            
            CreateAppointmentResponse createAppointmentResponse = _mapper.Map<CreateAppointmentResponse>(appointment);
            createAppointmentResponse.ServiceName = service?.Name;

            return new ServiceResult<CreateAppointmentResponse>().Success(createAppointmentResponse);        
    }

    public async Task<ServiceResult<UpdateAppointmentResponse>> UpdateAsync(UpdateAppointmentRequest request)
    {
        var entity = await _appointmentRepository.GetByAppointmentIdAsync(request.Id);
        if (entity == null)
        { return new ServiceResult<UpdateAppointmentResponse>().NotFound(Messages.NotFound); }

        entity.ServiceId = request.ServiceId;
        entity.AppointmentDate = request.AppointmentDate;
        entity.Status = request.Status;

        _appointmentRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        var updatedService = await _serviceRepository.GetByIdAsync(request.ServiceId);
        if (updatedService == null)
        {
            return new ServiceResult<UpdateAppointmentResponse>().Fail(Messages.NotFound);
        }

        var response = new UpdateAppointmentResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ServiceName = updatedService.Name, 
            AppointmentDate = entity.AppointmentDate,
            Status = entity.Status
        };

        return new ServiceResult<UpdateAppointmentResponse>().Success(response);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        
        var entity = await _appointmentRepository.GetByIdAsync(id);

        if (entity == null)
        { return new ServiceResult<bool>().NotFound(Messages.NotFound); }

        entity.IsDelete = true;
        entity.DeletedDate = DateTime.UtcNow;
        _appointmentRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResult<bool>().Success(true);
    }

    public async Task<ServiceResult<List<AppointmentDto>>> GetAll()
    {
        List<AppointmentDto> appointments = await _appointmentRepository.GetAllAppointmentAsync();
        return new ServiceResult<List<AppointmentDto>>().Success(appointments);
    }

    public async Task<ServiceResult<AppointmentDto?>> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByAppointmentIdAsync(id);
        if (appointment == null)
        {
            return new ServiceResult<AppointmentDto?>().Fail(Messages.NotFound, System.Net.HttpStatusCode.NotFound);
        }

        var appointmentDto = new AppointmentDto
        {
            Id = appointment.Id,
            UserId = appointment.UserId,
            UserName = appointment.User?.UserName,
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service?.Name,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,            
            CreatedDate = appointment.CreatedDate
        };

        return new ServiceResult<AppointmentDto?>().Success(appointmentDto);
    }

    public async Task<ServiceResult<bool>> SoftDeleteAsync(Guid id)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

        bool isCustomer = _httpContextAccessor.HttpContext?.User.IsInRole(nameof(UserRoles.Customer)) ?? false;
        var entity = await _appointmentRepository.GetByIdAsync(id);
        
        if (entity == null)
            return new ServiceResult<bool>().NotFound(Messages.NotFound);

        if (isCustomer && (userId == null || entity.UserId != userId))
            return new ServiceResult<bool>().Fail(Messages.YouCanOnlyDeleteYourOwnAppointments);
       
        entity.IsDelete = true;
        entity.DeletedDate = DateTime.UtcNow;
        _appointmentRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResult<bool>().Success(true);
    }

    public async Task<ServiceResult<List<AppointmentDto>>> Where(Expression<Func<Appointment, bool>> expression)
    {
        var appointments = await _appointmentRepository
            .Where(expression)
            .Include(a => a.Service)
            .Include(a => a.User)   
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = a.User.UserName,
            ServiceId = a.ServiceId,
            ServiceName = a.Service.Name,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status,           
            CreatedDate = a.CreatedDate
        }).ToList();

        return new ServiceResult<List<AppointmentDto>>().Success(appointmentDtos);
    }

    public async Task<ServiceResult<UpdateAppointmentStatusResponse>> UpdateStatusAsync(UpdateAppointmentStatusRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
        if (appointment == null)
        {
            return new ServiceResult<UpdateAppointmentStatusResponse>().Fail(Messages.NotFound);
        }

        if (!Enum.IsDefined(typeof(AppointmentStatus), request.Status))
        {
            return new ServiceResult<UpdateAppointmentStatusResponse>().Fail(Messages.InvalidStatus);
        }

        appointment.Status = (AppointmentStatus)request.Status;
        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResult<UpdateAppointmentStatusResponse>().Success(new UpdateAppointmentStatusResponse
        {
            Id = appointment.Id,
            Status = appointment.Status.ToString()
        });
    }

    public async Task<ServiceResult<AppointmentsViewModel>> GetAppointmentsForUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return new ServiceResult<AppointmentsViewModel>().Fail(Messages.UserInformationCouldNotBeFound);

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

        bool isCustomer = httpContext.User.IsInRole(nameof(UserRoles.Customer));
        bool isAdmin = httpContext.User.IsInRole(nameof(UserRoles.Admin));

        if (isCustomer)
        {
            if (userId == null)
                return new ServiceResult<AppointmentsViewModel>().Fail(Messages.UserIDNotFound);

            var appointments = await _appointmentRepository.GetAllByUserAppointmentAsync(userId.Value);
            
            var appointmentDtos = appointments.Select(a => _mapper.Map<AppointmentDto>(a)).ToList();
            return new ServiceResult<AppointmentsViewModel>().Success(new AppointmentsViewModel
            {
                IsCustomer = isCustomer,
                IsAdmin = isAdmin,
                Appointments = appointmentDtos
            });
        }

        if (isAdmin)
        {
            var appointments = await _appointmentRepository.GetAllAppointmentAsync();               

            var appointmentDtos = appointments.Select(a => _mapper.Map<AppointmentDto>(a)).ToList();
            return new ServiceResult<AppointmentsViewModel>().Success(new AppointmentsViewModel
            {
                IsCustomer = isCustomer,
                IsAdmin = isAdmin,
                Appointments = appointmentDtos
            });
        }

        return new ServiceResult<AppointmentsViewModel>().Fail(Messages.AppointmentsCouldNotBeLoaded);
    }
}