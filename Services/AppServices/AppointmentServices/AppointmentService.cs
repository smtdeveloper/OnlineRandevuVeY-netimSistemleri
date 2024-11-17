using AutoMapper;
using Entities.DTOs.Appointment;
using Entities.DTOs.UserRole;
using Entities.Enums;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.AppointmentDal;
using Repositories.RepositoriesDal.ServiceDal;
using Repositories.UnitOfWorks;
using Services.BusinessRules;
using System.Linq.Expressions;

namespace Services.AppServices.AppointmentServices;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
         
    public AppointmentService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork, IMapper mapper, IServiceRepository serviceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CreateAppointmentResponse>> CreateAsync(CreateAppointmentRequest request)
    {
        var appointment = new Appointment
        {
            UserId = request.UserId,
            ServiceId = request.ServiceId,
            AppointmentDate = request.AppointmentDate,
            Status = AppointmentStatus.Pending,
            IsDelete = false,
            CreatedDate = DateTime.UtcNow
        };

        await _appointmentRepository.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        // ServiceName'i Service tablosundan getirme
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);

        return new ServiceResult<CreateAppointmentResponse>().Success(new CreateAppointmentResponse
        {
            Id = appointment.Id,
            ServiceName = service?.Name, // Burada ServiceName atanıyor
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status.ToString()
        });
    }




    public async Task<ServiceResult<UpdateAppointmentResponse>> UpdateAsync(UpdateAppointmentRequest request)
    {
        // Güncellenmesi gereken randevuyu getir
        var entity = await _appointmentRepository.GetByAppointmentIdAsync(request.Id); // Service dahil olacak
        var businessRuleResult = GenericBusinessRules.CheckEntityNotNull(entity, nameof(Appointment));

        if (businessRuleResult != null)
        {
            return new ServiceResult<UpdateAppointmentResponse>().Fail(businessRuleResult.ErrorMessage);
        }

        // Güncelleme işlemleri
        entity.ServiceId = request.ServiceId;
        entity.AppointmentDate = request.AppointmentDate;
        entity.Status = request.Status;

        // Randevu güncellendi
        _appointmentRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        // Güncellenen Service bilgisi için Repository üzerinden sorgu
        var updatedService = await _appointmentRepository.GetServiceByIdAsync(request.ServiceId);
        if (updatedService == null)
        {
            return new ServiceResult<UpdateAppointmentResponse>().Fail("Hizmet bilgisi bulunamadı.");
        }

        // Güncelleme işlemi tamamlandıktan sonra response DTO'su oluştur
        var response = new UpdateAppointmentResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ServiceName = updatedService.Name, // Güncellenmiş ServiceName
            AppointmentDate = entity.AppointmentDate,
            Status = entity.Status
        };

        return new ServiceResult<UpdateAppointmentResponse>().Success(response);
    }



    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        
        var entity = await _appointmentRepository.GetByIdAsync(id);
        var businessRuleResult = GenericBusinessRules.CheckEntityNotNull(entity, nameof(Appointment));

        if (businessRuleResult != null)
        {
            return businessRuleResult;
        }

        entity.IsDelete = true;
        entity.DeletedDate = DateTime.UtcNow;
        _appointmentRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResult<bool>().Success(true);
    }

    public async Task<ServiceResult<List<AppointmentDto>>> GetAll()
    {
        var appointments = await _appointmentRepository.GetAll().Where(a => a.IsDelete == false)
            .Include(a => a.User) // Kullanıcı bilgisine erişiyoruz
            .Include(a => a.Service) // Servis bilgisine erişiyoruz
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = a.User.UserName, // Kullanıcı adı ekleniyor
            ServiceName = a.Service.Name,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status.ToString(),
        }).ToList();

        return new ServiceResult<List<AppointmentDto>>().Success(appointmentDtos);
    }


    public async Task<ServiceResult<AppointmentDto?>> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByAppointmentIdAsync(id); // Service dahil olacak şekilde ilişki yüklenir

        if (appointment == null)
        {
            return new ServiceResult<AppointmentDto?>().Fail("Appointment not found", System.Net.HttpStatusCode.NotFound);
        }

        // AppointmentDto içerisine Service bilgilerini de map eder
        var appointmentDto = new AppointmentDto
        {
            Id = appointment.Id,
            UserId = appointment.UserId,
            UserName = appointment.User?.UserName ?? "Unknown",
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service?.Name ?? "Unknown",
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status.ToString(),
            IsDelete = appointment.IsDelete,
            CreatedDate = appointment.CreatedDate
        };

        return new ServiceResult<AppointmentDto?>().Success(appointmentDto);
    }



    public async Task<ServiceResult<bool>> SoftDeleteAsync(Guid id)
    {
        
        var entity = await _appointmentRepository.GetByIdAsync(id);
        var businessRuleResult = GenericBusinessRules.CheckEntityNotNull(entity, nameof(Appointment));

        if (businessRuleResult != null)
        {
            return businessRuleResult;
        }

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
            .Include(a => a.Service) // Servis ile ilişkilendirme
            .Include(a => a.User)    // Kullanıcı ile ilişkilendirme
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = a.User != null ? a.User.UserName : "Unknown", // Kullanıcı adı
            ServiceId = a.ServiceId,
            ServiceName = a.Service.Name, // Servis adı
            AppointmentDate = a.AppointmentDate,
            Status = a.Status.ToString(),
            IsDelete = a.IsDelete,
            CreatedDate = a.CreatedDate
        }).ToList();

        return new ServiceResult<List<AppointmentDto>>().Success(appointmentDtos);
    }

    public async Task<ServiceResult<UpdateAppointmentStatusResponse>> UpdateStatusAsync(UpdateAppointmentStatusRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
        if (appointment == null)
        {
            return new ServiceResult<UpdateAppointmentStatusResponse>().Fail("Randevu bulunamadı.");
        }

        if (!Enum.IsDefined(typeof(AppointmentStatus), request.Status))
        {
            return new ServiceResult<UpdateAppointmentStatusResponse>().Fail("Geçersiz durum.");
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


}