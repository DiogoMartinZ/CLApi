using CLApi.Data;
using CLApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

[Route("api/auth")]
[ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.PasswordHash == loginDto.PasswordHash);

            if (user == null)
            {
                return Unauthorized(new { message = "Utilizador ou palavra-passe inválidos." });
            }

            return Ok(new { success = true, message = "Login efetuado com sucesso!" });
        }
    }

