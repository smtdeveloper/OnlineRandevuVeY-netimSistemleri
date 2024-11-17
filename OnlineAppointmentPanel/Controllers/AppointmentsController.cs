using AutoMapper;
using Entities.DTOs.Appointment;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.AppServices.AppointmentServices;
using Services.AppServices.ServiceServices;
using System.Security.Claims;

namespace OnlineAppointmentPanel.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public AppointmentsController(IAppointmentService appointmentService, IServiceService serviceService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _serviceService = serviceService;
            _mapper = mapper;
        }

        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Customer))]
        public async Task<IActionResult> Index()
        {
            var result = await _appointmentService.GetAppointmentsForUserAsync();
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = string.Join(",", result.ErrorMessage);
                return View();
            }
            ViewBag.IsCustomer = result.Data.IsCustomer;
            ViewBag.IsAdmin = result.Data.IsAdmin;
            return View(result.Data);
        }

        [HttpGet]
        [Route("Appointments/GetById/{id}")]
        public async Task<JsonResult> GetById(Guid id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Json(new
                {
                    success = true,
                    message = "Randevu başarıyla oluşturuldu.",
                    data = result.Data
                });
            }

            return Json(new
            {
                success = false,
                message = "Bulunamadı!"
            });

        }

        [HttpGet]
        [Route("Appointments/Detail/{id}")]
        public async Task<IActionResult> Detail([FromRoute] Guid id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (result.IsSuccess && result.Data != null)
            {
                return View(result.Data);
            }

            ViewBag.ErrorMessage = string.Join(",", result.ErrorMessage);
            return View("Error");
        }

        [HttpGet]
        [Route("Appointments/GetServices")]
        public async Task<IActionResult> GetServices()
        {
            var result = await _serviceService.Where(s => s.IsDelete == false);
            if (result.IsSuccess && result.Data != null)
            {
                var services = result.Data.Select(s => new
                {
                    id = s.Id,
                    name = s.Name
                }).ToList();

                return Json(new { success = true, services });
            }

            return Json(new { success = false, errorMessage = "Servisler yüklenemedi." });
        }

        [HttpGet]
        public IActionResult GetStatuses()
        {
            var statuses = Enum.GetValues(typeof(AppointmentStatus))
                .Cast<AppointmentStatus>()
                .Where(status => status != AppointmentStatus.Unknown)
                .Select(status => new
                {
                    Id = (int)status,
                    Name = status.ToString()
                }).ToList();

            return Json(new { success = true, statuses });
        }

        [Authorize(Roles = nameof(UserRoles.Customer))]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errorMessage = errors[0], errors });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Json(new { success = false, errorMessage = "Kullanıcı bilgisi bulunamadı, lütfen tekrar giriş yapınız!" });
            }

            request.UserId = Guid.Parse(userIdClaim);

            var result = await _appointmentService.CreateAsync(request);
            if (result.IsSuccess)
            {
                var createdAppointment = result.Data;
                return Json(new
                {
                    success = true,
                    message = "Randevu başarıyla oluşturuldu.",
                    appointment = new
                    {
                        Id = createdAppointment.Id,
                        UserName = User.Identity.Name,
                        ServiceName = createdAppointment.ServiceName,
                        AppointmentDate = createdAppointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                        Status = createdAppointment.Status
                    }
                });

            }

            return Json(new { success = false, errorMessage = string.Join(",", result.ErrorMessage) });
        }

        [Authorize(Roles = nameof(UserRoles.Customer))]
        [HttpPost]
        public async Task<JsonResult> Update(UpdateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errorMessage = errors[0], errors });
            }

            var originalData = await _appointmentService.GetByIdAsync(request.Id);  
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole(nameof(UserRoles.Customer)) &&
                (userIdClaim == null || originalData.Data.UserId.ToString() != userIdClaim))
            {
                return Json(new { success = false, errorMessage = "Bu randevuyu güncelleme yetkiniz yok." });
            }


            if (originalData.Data.Status != AppointmentStatus.Pending)
            {
                return Json(new { success = false, errorMessage = "Sadece beklemede durumundaki randevularınızı güncelleyebilirsiniz." });
            }
            request.Status = AppointmentStatus.Pending;


            var result = await _appointmentService.UpdateAsync(request);
            if (result.IsSuccess)
            {
                var updatedAppointment = result.Data;
                return Json(new
                {
                    success = true,
                    message = "Randevu başarıyla güncellendi.",
                    appointment = new
                    {
                        Id = updatedAppointment.Id,
                        UserName = User.Identity.Name,
                        UserId = updatedAppointment.UserId,
                        ServiceName = updatedAppointment.ServiceName,
                        AppointmentDate = updatedAppointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                        Status = updatedAppointment.Status.ToString()
                    }
                });
            }


            return Json(new { success = false, errorMessage = string.Join(",", result.ErrorMessage) });
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(UpdateAppointmentStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errorMessage = errors[0], errors });
            }

            var result = await _appointmentService.UpdateStatusAsync(request);
            if (result.IsSuccess)
            {
                return Json(new
                {
                    success = true,
                    message = "Durum başarıyla güncellendi.",
                    appointment = new
                    {
                        id = result.Data.Id,
                        status = result.Data.Status
                    }
                });
            }

            return Json(new { success = false, errorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _appointmentService.SoftDeleteAsync(id);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Randevu başarıyla silindi.", id = id });
            }

            return Json(new { success = false, errorMessage = string.Join(",", result.ErrorMessage) });
        }

    }
}