using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SistemaAcademico.App_Start;
using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaAcademico.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
         
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                // Validación del ModelState
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new LoginResultViewModel
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        RemainingAttempts = null
                    });
                }

                // Buscar usuario por email
                var user = await UserManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    return Json(new LoginResultViewModel
                    {
                        Success = false,
                        Message = "Credenciales inválidas. Verifique su correo electrónico y contraseña."
                    });
                }
                 

                // Verificar si está bloqueado
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    var lockoutMinutes = int.Parse(
                        ConfigurationManager.AppSettings["Authentication:LockoutMinutes"] ?? "15");

                    return Json(new LoginResultViewModel
                    {
                        Success = false,
                        Message = $"Su cuenta ha sido bloqueada por {lockoutMinutes} minutos debido a múltiples intentos fallidos.",
                        IsLockedOut = true
                    });
                }

                // Intentar autenticación
                var result = await SignInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    shouldLockout: true );

                switch (result)
                {
                    case SignInStatus.Success:
                        // Verificar que tenga el rol de Docente
                        if (!await UserManager.IsInRoleAsync(user.Id, "Docente") && !await UserManager.IsInRoleAsync(user.Id, "Administrador"))
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                            return Json(new LoginResultViewModel
                            {
                                Success = false,
                                Message = "No tiene permisos para acceder al sistema."
                            });
                        }

                        // Login exitoso
                        var redirectUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                            ? returnUrl
                            : Url.Action("Index", "Home");

                        return Json(new LoginResultViewModel
                        {
                            Success = true,
                            Message = "Autenticación exitosa. Redirigiendo...",
                            RedirectUrl = redirectUrl
                        });

                    case SignInStatus.LockedOut:
                        var lockoutTime = int.Parse(
                            ConfigurationManager.AppSettings["Authentication:LockoutMinutes"] ?? "15");

                        return Json(new LoginResultViewModel
                        {
                            Success = false,
                            Message = $"Cuenta bloqueada por {lockoutTime} minutos debido a múltiples intentos fallidos.",
                            IsLockedOut = true
                        });

                    case SignInStatus.Failure:
                    default:
                        // Obtener intentos restantes
                        var maxAttempts = int.Parse(
                            ConfigurationManager.AppSettings["Authentication:MaxLoginAttempts"] ?? "5");
                        var failedAttempts = await UserManager.GetAccessFailedCountAsync(user.Id);
                        var remainingAttempts = maxAttempts - failedAttempts;

                        string message;
                        if (remainingAttempts > 0)
                        {
                            message = $"Credenciales inválidas. Intentos restantes: {remainingAttempts}";
                        }
                        else
                        {
                            message = "Credenciales inválidas.";
                        }

                        return Json(new LoginResultViewModel
                        {
                            Success = false,
                            Message = message,
                            RemainingAttempts = remainingAttempts > 0 ? remainingAttempts : (int?)null
                        });
                }
            }
            catch (Exception ex)
            {
                // Log del error (implementar logging según necesidades)
                System.Diagnostics.Debug.WriteLine($"Error en Login: {ex.Message}");

                return Json(new LoginResultViewModel
                {
                    Success = false,
                    Message = "Error inesperado al procesar la solicitud. Intente nuevamente."
                });
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar un correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar la cuenta", "Para confirmar su cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
