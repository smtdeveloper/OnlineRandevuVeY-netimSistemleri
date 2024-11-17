using AutoMapper;
using Entities.DTOs.Auth;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.AppServices.UserServices;

namespace OnlineAppointmentPanel.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        

        public UsersController(IUserService userService)
        {
            _userService = userService;           
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        public async Task<IActionResult> Index()      
        {
            var result = await _userService.GetAll();
            if (result.IsSuccess)
            {
                return View(result.Data);
            }
            return View("Error", result.ErrorMessage);
        }

        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Customer))]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return View(result.Data);
            }
            return View("Error", result.ErrorMessage);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var result = await _userService.CreateAsync(request);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, string.Join(",", result.ErrorMessage));
            return View(request);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpDelete]
        [Route("/Users/Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
            }
            return Json(new { success = false, errorMessage = result.ErrorMessage });
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Error", result.ErrorMessage);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet]
        public async Task<IActionResult> AssignRole(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return View("Error", result.ErrorMessage);
            }

            var assignedRoles = result.Data.Roles;

            var model = new AssignRoleRequest
            {
                UserId = id,
                AvailableRoles = Enum.GetValues(typeof(UserRoles))
                                     .Cast<UserRoles>()
                                     .Where(role => role != UserRoles.Unknown)
                                     .Select(role => new RoleDto
                                     {
                                         Id = (int)role,
                                         Name = role.ToString()
                                     })
                                     .ToList(),

                AssignedRoles = assignedRoles.Select(r => new RoleDto
                {
                    Id = (int)r,
                    Name = r.ToString()
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                request.AvailableRoles = Enum.GetValues(typeof(UserRoles))
                                              .Cast<UserRoles>()
                                              .Where(role => role != UserRoles.Unknown)
                                              .Select(role => new RoleDto
                                              {
                                                  Id = (int)role,
                                                  Name = role.ToString()
                                              })
                                              .ToList();

                return View(request);
            }

            var userResult = await _userService.GetByIdAsync(request.UserId);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bilgisi alınamadı.");
                return View(request);
            }

            if (userResult.Data.Roles.Contains(request.SelectedRole))
            {
                TempData["ErrorMessage"] = "Bu rol zaten atanmış durumda.";
                return RedirectToAction("AssignRole", new { id = request.UserId });
            }

            var result = await _userService.AssignRoleAsync(request.UserId, request.SelectedRole);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Rol başarıyla atandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, string.Join(",", result.ErrorMessage));
            return View(request);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> RemoveRole(Guid userId, UserRoles role)
        {
            var removeRoleResult = await _userService.RemoveRoleAsync(userId, role);
            if (removeRoleResult.IsSuccess)
            {
                TempData["SuccessMessage"] = "Rol başarıyla geri çekildi.";
            }
            else
            {
                TempData["ErrorMessage"] = removeRoleResult.ErrorMessage;
            }

            return RedirectToAction("AssignRole", new { id = userId });
        }

    }
}
