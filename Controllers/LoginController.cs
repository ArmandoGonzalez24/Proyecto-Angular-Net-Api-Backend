using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductCategoryCrud.Data;
using ProductCategoryCrud.Helpers;
using ProductCategoryCrud.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductCategoryCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public LoginController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            // Verificar si el nombre de usuario ya existe
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("El nombre de usuario ya existe.");
            }

            // Verificar que el rol exista
            var role = await _context.Roles.FindAsync(registerDto.RoleId);
            if (role == null)
            {
                return BadRequest("El rol especificado no existe.");
            }

            // Crear el usuario
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password), // Hashear la contraseña
                RoleId = registerDto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //return Ok("Usuario registrado exitosamente.");
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            // Verificar si el usuario existe
            var dbUser = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == loginDto.Username);

            if (dbUser == null || !PasswordHasher.VerifyPassword(loginDto.Password, dbUser.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas");
            }

            // Generar el token JWT
            var token = GenerateJwtToken(dbUser);
            return Ok(new { Token = token });
        }


        private string GenerateJwtToken(User user)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Nombre de usuario
        new Claim(ClaimTypes.Name, user.Username), // Para User.Identity.Name
        new Claim(ClaimTypes.Role, user.Role.Name), // Rol del usuario
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único del token
    };

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationMinutes),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

    }
}
