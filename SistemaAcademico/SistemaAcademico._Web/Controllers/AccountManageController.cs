using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data.Entities;
using SistemaAcademico.Data.Context;
using SistemaAcademico._Web.Models.ViewModels;

namespace SistemaAcademico._Web.Controllers
{
    [Authorize]
    public class AccountManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public AccountManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        // GET: /AccountManage/Index
        public async Task<IActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su contraseña ha sido cambiada exitosamente."
                : message == ManageMessageId.UpdateProfileSuccess ? "Su perfil ha sido actualizado exitosamente."
                : message == ManageMessageId.Error ? "Ha ocurrido un error."
                : "";

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roleNames = await _userManager.GetRolesAsync(user);

            var model = new ManageProfileViewModel();
            var docente = await _db.Docente
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (docente != null)
            {
                model.Nombre = docente.Nombre;
                model.Apellidos = docente.Apellidos;
                model.FechaCreacion = docente.Fec_Registro;
            }
            else
            {
                model.Nombre = "Admin";
                model.Apellidos = "Sin apellidos";
                model.FechaCreacion = DateTime.Now;
            }

            model.Email = user.Email ?? string.Empty;
            //model.UltimaConexion = user.UltimaConexion ?? DateTime.Now;
            model.Rol = roleNames.FirstOrDefault();

            return View(model);
        }

        // POST: /AccountManage/UpdateProfile (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateProfile(ManageProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Datos inválidos",
                        errors
                    });
                }

                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Usuario no encontrado"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Usuario no encontrado"
                    });
                }

                var docente = await _db.Docente
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                if (docente == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Docente no encontrado"
                    });
                }

                docente.Nombre = model.Nombre.Trim();
                docente.Apellidos = model.Apellidos.Trim();

                _db.Entry(docente).State = EntityState.Modified;
                var resultUpdateDocente = await _db.SaveChangesAsync();

                if (resultUpdateDocente <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error al actualizar el perfil del docente"
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Perfil actualizado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error inesperado: " + ex.Message
                });
            }
        }

        // GET: /AccountManage/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /AccountManage/ChangePassword (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Datos inválidos",
                        errors
                    });
                }

                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Usuario no encontrado"
                    });
                }

                var result = await _userManager.ChangePasswordAsync(
                    await _userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException(),
                    model.CurrentPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Actualizar el security stamp para invalidar otros tokens
                        await _userManager.UpdateSecurityStampAsync(user);

                        // Refrescar el sign-in
                        await _signInManager.RefreshSignInAsync(user);
                    }

                    return Json(new
                    {
                        success = true,
                        message = "Contraseña cambiada exitosamente"
                    });
                }

                // Interpretar errores comunes
                var errorMessage = "Error al cambiar la contraseña";

                if (result.Errors.Any(e => e.Code.Contains("PasswordMismatch")))
                {
                    errorMessage = "La contraseña actual es incorrecta";
                }

                return Json(new
                {
                    success = false,
                    message = errorMessage,
                    errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error inesperado: " + ex.Message
                });
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            UpdateProfileSuccess,
            Error
        }
    }
}