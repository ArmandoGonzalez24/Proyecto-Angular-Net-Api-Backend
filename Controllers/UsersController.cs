using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategoryCrud.Data;
using ProductCategoryCrud.Helpers;
using ProductCategoryCrud.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductCategoryCrud.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Solo Admin puede gestionar usuarios
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Verificar si el nombre de usuario ya existe
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("El nombre de usuario ya existe.");
            }

            // Hashear la contraseña antes de almacenarla
            user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("El usuario no está autenticado.");
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                Role = user.Role?.Name
            });
        }

       

        // Otros métodos para PUT, DELETE, etc.
    }
}
