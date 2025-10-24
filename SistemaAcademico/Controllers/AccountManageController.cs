using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SistemaAcademico.App_Start;
using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using SistemaAcademico.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaAcademico.Controllers
{
    [Authorize]
    public class AccountManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db ;

        public AccountManageController()
        {
        }

        public AccountManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext db)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AppDbContext = db;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // <summary>
        /// Obtiene  configuracion del repositorio actual
        /// </summary> 
        public ApplicationDbContext AppDbContext
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
           
        }
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su contraseña ha sido cambiada exitosamente."
                : message == ManageMessageId.UpdateProfileSuccess ? "Su perfil ha sido actualizado exitosamente."
                : message == ManageMessageId.Error ? "Ha ocurrido un error."
                : "";
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var roleNames = await UserManager.GetRolesAsync(userId);



            var model = new ManageProfileViewModel();
            var docente = AppDbContext.Docente
                                    .AsNoTracking()
                                    .FirstOrDefault(d => d.UserId == userId);

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

            model.Email = user.Email;  
            model.UltimaConexion = DateTime.Now;
            model.Rol = roleNames.FirstOrDefault(); 
            return View(model);

        }
        // =============================================
        // POST: /Manage/UpdateProfile (AJAX)
        // =============================================
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
                        errors = errors
                    });
                }

                var userId = User.Identity.GetUserId();
                var user = await UserManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Usuario no encontrado"
                    });
                }
                var docente = AppDbContext.Docente
                    .AsNoTracking()
                                    .FirstOrDefault(d => d.UserId == userId); 
                docente.Nombre = model.Nombre.Trim();
                docente.Apellidos = model.Apellidos.Trim();
                AppDbContext.Entry(docente).State = System.Data.Entity.EntityState.Modified;
                var resultUpdateDocente = AppDbContext.SaveChanges();
                if (resultUpdateDocente <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error al actualizar el perfil del docente"
                    });
                }else
                {
                    return Json(new
                    {
                        success = true,
                        message = "Perfil actualizado exitosamente"
                    });
                }

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

        // =============================================
        // GET: /Manage/ChangePassword
        // =============================================
        public ActionResult ChangePassword()
        {
            return View();
        }
        // =============================================
        // POST: /Manage/ChangePassword (AJAX)
        // =============================================
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
                        errors = errors
                    });
                }

                var userId = User.Identity.GetUserId();
                var result = await UserManager.ChangePasswordAsync(
                    userId,
                    model.CurrentPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Actualizar el security stamp para invalidar otros tokens
                        await UserManager.UpdateSecurityStampAsync(userId);
                    }

                    return Json(new
                    {
                        success = true,
                        message = "Contraseña cambiada exitosamente"
                    });
                }
                else
                {
                    // Interpretar errores comunes
                    var errorMessage = "Error al cambiar la contraseña";

                    if (result.Errors.Any(e => e.Contains("Incorrect password")))
                    {
                        errorMessage = "La contraseña actual es incorrecta";
                    }

                    return Json(new
                    {
                        success = false,
                        message = errorMessage,
                        errors = result.Errors.ToList()
                    });
                }
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



        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            UpdateProfileSuccess,
            Error
        }

        #endregion
    }
}