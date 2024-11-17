using AutoMapper;
using Entities.DTOs.Appointment;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            ViewBag.IsCustomer = User.IsInRole(nameof(UserRoles.Customer));
            ViewBag.IsAdmin = User.IsInRole(nameof(UserRoles.Admin));
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Eğer kullanıcı Customer ise yalnızca kendi randevularını görsün
            if (User.IsInRole(nameof(UserRoles.Customer)))
            {
                if (userIdClaim == null)
                {
                    ViewBag.ErrorMessage = "Kullanıcı kimliği bulunamadı.";
                    return View();
                }

                var userId = Guid.Parse(userIdClaim);
                var customerResult = await _appointmentService.Where(_user => _user.UserId == userId && _user.IsDelete == false);
                if (customerResult.IsSuccess)
                {
                    return View(customerResult.Data);
                }

                ViewBag.ErrorMessage = string.Join(",", customerResult.ErrorMessage);
                return View();
            }

            // Eğer kullanıcı Admin ise tüm randevuları görsün
            if (User.IsInRole(nameof(UserRoles.Admin)))
            {
                var adminResult = await _appointmentService.GetAll();
                if (adminResult.IsSuccess)
                {
                    return View(adminResult.Data);
                }

                ViewBag.ErrorMessage = string.Join(",", adminResult.ErrorMessage);
                return View();
            }

            // Eğer herhangi bir rol eşleşmezse hata mesajı döner
            ViewBag.ErrorMessage = "Randevular görüntülenemedi.";
            return View();
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
            ViewBag.ErrorMessage = string.Join(",", result.ErrorMessage);

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
                return View(result.Data); // DTO'yu View'e gönderiyoruz
            }

            // Eğer hata varsa, hata mesajı ile birlikte hata View'ini döndür
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var result = await _serviceService.Where(_service => _service.IsDelete == false);

            if (!result.IsSuccess || result.Data == null)
            {
                ViewBag.ErrorMessage = "Servisler yüklenemedi.";
                return View("Error");
            }

            ViewBag.Services = new SelectList(result.Data, "Id", "Name");

            return View();
        }

        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Customer))]
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                ViewBag.ErrorMessage = string.Join(",", result.ErrorMessage);
                return View("Error");
            }

            var servicesResult = await _serviceService.GetAll();
            if (!servicesResult.IsSuccess || servicesResult.Data == null)
            {
                ViewBag.ErrorMessage = "Servisler yüklenemedi.";
                return View("Error");
            }

            ViewBag.Services = new SelectList(servicesResult.Data, "Id", "Name");

            // Kullanıcının Admin olup olmadığını belirle
            ViewBag.IsAdmin = User.IsInRole(nameof(UserRoles.Admin));

            var updateRequest = _mapper.Map<UpdateAppointmentRequest>(result.Data);
            return View(updateRequest);
        }

        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Customer))]
        [HttpPost]
        public async Task<JsonResult> Update(UpdateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errorMessage = errors[0], errors });
            }

            var originalData = await _appointmentService.GetByIdAsync(request.Id);
            if (!originalData.IsSuccess || originalData.Data == null)
            {
                return Json(new { success = false, errorMessage = "Orijinal randevu bulunamadı." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole(nameof(UserRoles.Customer)) &&
                (userIdClaim == null || originalData.Data.UserId.ToString() != userIdClaim))
            {
                return Json(new { success = false, errorMessage = "Bu randevuyu güncelleme yetkiniz yok." });
            }

            if (User.IsInRole(nameof(UserRoles.Customer)))
            {
                if (!Enum.TryParse(originalData.Data.Status, out AppointmentStatus status) || status != AppointmentStatus.Pending)
                {
                    return Json(new { success = false, errorMessage = "Sadece beklemede durumundaki randevularınızı güncelleyebilirsiniz." });
                }
                request.Status = AppointmentStatus.Pending;
            }

            if (User.IsInRole(nameof(UserRoles.Admin)))
            {
                request.ServiceId = originalData.Data.ServiceId;
                request.AppointmentDate = originalData.Data.AppointmentDate;
            }

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
            var originalData = await _appointmentService.GetByIdAsync(id);
            if (!originalData.IsSuccess || originalData.Data == null)
            {
                return Json(new { success = false, errorMessage = "Orijinal randevu bulunamadı." });
            }

            // Kullanıcının sadece kendi randevusunu güncelleyebilmesi için kontrol
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole(nameof(UserRoles.Customer)) && (userIdClaim == null || originalData.Data.UserId.ToString() != userIdClaim))
            {
                return Json(new { success = false, errorMessage = "Sadeve kendi randevularınızı silebilirsiniz." });
            }

            var result = await _appointmentService.SoftDeleteAsync(id);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Randevu başarıyla silindi.", id = id });
            }

            return Json(new { success = false, errorMessage = string.Join(",", result.ErrorMessage) });
        }

    }
}