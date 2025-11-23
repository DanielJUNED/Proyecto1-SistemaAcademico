using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SistemaAcademico.Data.Entities;
using System.Text;

namespace SistemaAcademico.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            // 1️ Validar usuario
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Unauthorized("Credenciales incorrectas");

            // 2️ Validar password
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Credenciales incorrectas");

            // 3️ Claims principales
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            // 4️ Agregar roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 5️ Crear la clave
            var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var key = new SymmetricSecurityKey(keyBytes);

            // 6️ Credenciales (HS256)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 7️ Generar el token JWT
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            // 8️ Devolver token al MVC
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
       public class LoginDTO
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
 }