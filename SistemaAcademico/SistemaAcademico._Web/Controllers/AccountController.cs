using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.Data.Entities;
using SistemaAcademico._Web.Models.ViewModels;

namespace SistemaAcademico.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model, string? returnUrl = null)
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
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    return Json(new LoginResultViewModel
                    {
                        Success = false,
                        Message = "Credenciales inválidas. Verifique su correo electrónico y contraseña."
                    });
                }

                // Verificar si está bloqueado
                if (await _userManager.IsLockedOutAsync(user))
                {
                    var lockoutMinutes = int.Parse(
                        _configuration["Authentication:LockoutMinutes"] ?? "15");

                    return Json(new LoginResultViewModel
                    {
                        Success = false,
                        Message = $"Su cuenta ha sido bloqueada por {lockoutMinutes} minutos debido a múltiples intentos fallidos.",
                        IsLockedOut = true
                    });
                }

                // Intentar autenticación
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Verificar que tenga el rol de Docente o Administrador
                    if (!await _userManager.IsInRoleAsync(user, "Docente") &&
                        !await _userManager.IsInRoleAsync(user, "Administrador"))
                    {
                        await _signInManager.SignOutAsync();

                        return Json(new LoginResultViewModel
                        {
                            Success = false,
                            Message = "No tiene permisos para acceder al sistema."
                        });
                    }

                    // Actualizar última conexión
                    //user.UltimaConexion = DateTime.Now;
                    await _userManager.UpdateAsync(user);

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
                }

                if (result.IsLockedOut)
                {
                    var lockoutTime = int.Parse(
                        _configuration["Authentication:LockoutMinutes"] ?? "15");

                    return Json(new LoginResultViewModel
                    {
                        Success = false,
                        Message = $"Cuenta bloqueada por {lockoutTime} minutos debido a múltiples intentos fallidos.",
                        IsLockedOut = true
                    });
                }

                // Obtener intentos restantes
                var maxAttempts = int.Parse(
                    _configuration["Authentication:MaxLoginAttempts"] ?? "5");
                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
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
                    RemainingAttempts = remainingAttempts > 0 ? remainingAttempts : null
                });
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error en Login: {ex.Message}");

                return Json(new LoginResultViewModel
                {
                    Success = false,
                    Message = "Error inesperado al procesar la solicitud. Intente nuevamente."
                });
            }
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}