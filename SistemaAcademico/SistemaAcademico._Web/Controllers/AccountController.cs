using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SistemaAcademico._Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
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
               /* if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Datos inválidos" });
                }

                // ================================
                // 1️⃣ VALIDAR CON IDENTITY
                // ================================
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    return Json(new { success = false, message = "Credenciales inválidas" });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Credenciales inválidas" });
                }

                // ================================
                // 2️⃣ OBTENER TOKEN JWT DE LA API
                // ================================
                var client = _httpClientFactory.CreateClient("API");

                var response = await client.PostAsJsonAsync("auth/login", new
                {
                    username = model.UserName,
                    password = model.Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "No se pudo obtener el token JWT" });
                }

                var apiResult = await response.Content.ReadFromJsonAsync<JwtLoginResponse>();

                if (apiResult == null || string.IsNullOrEmpty(apiResult.Token))
                {
                    return Json(new { success = false, message = "Token inválido" });
                }

                // ================================
                // 3️⃣ GUARDAR JWT EN COOKIE SEGURA
                // ================================
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(8)
                };

                Response.Cookies.Append("AuthToken", apiResult.Token, cookieOptions);

                // ================================
                // 4️⃣ CREAR SESIÓN LOCAL MVC (Claims)
                // ================================
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(apiResult.Token);

                var claims = jwtToken.Claims.ToList();

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddHours(8)
                    }
                );


                return Json(new
                {
                    success = true,
                    message = "Autenticación exitosa",
                    redirectUrl = Url.Action("Index", "Home")
                });*/
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

                    var client = _httpClientFactory.CreateClient("API");
                    // ViewModel → DTO
                    var dto = new BitacoraBaseDTO
                    {
                        UserId = user.Id,
                        Accion = SistemaAcademico.Data.Constantes.AccionesBitacora.Login,
                        Modulo = SistemaAcademico.Data.Constantes.ModulosBitacora.Autenticacion,
                        Descripcion = "Inicio de sesión del usuario " + user.UserName,
                        DireccionIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida",
                        Fec_Registro = DateTime.UtcNow
                    };

                    var jsonContent = JsonSerializer.Serialize(dto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("bitacora", content);

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
                var clientIntento = _httpClientFactory.CreateClient("API");
                // ViewModel → DTO
                var dtoIntento = new BitacoraBaseDTO
                {
                    UserId = user.Id,
                    Accion = SistemaAcademico.Data.Constantes.AccionesBitacora.LoginFallido,
                    Modulo = SistemaAcademico.Data.Constantes.ModulosBitacora.Autenticacion,
                    Descripcion = "Intento fallidos para el usuario " + user.UserName ,
                    DireccionIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida",
                    Fec_Registro = DateTime.UtcNow
                };

                var jsonContentIntento = JsonSerializer.Serialize(dtoIntento);
                var contentIntento = new StringContent(jsonContentIntento, Encoding.UTF8, "application/json");

                var responseIntento = await clientIntento.PostAsync("bitacora", contentIntento);
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
        public class JwtLoginResponse
        {
            public bool Success { get; set; }
            public string Token { get; set; } = "";
            public string? Message { get; set; }
            public List<string>? Roles { get; set; }
            public DateTime? Expiration { get; set; }
        }
    }
}