using ExpenseTracker.Data;
using ExpenseTracker.Models.DTO;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        //var cors = new EnableCorsAttribute();
       
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/users/register
        [EnableCors("CorsPolicy")]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterUserDto registerUserDto)
        {
            if (string.IsNullOrWhiteSpace(registerUserDto.Name) || string.IsNullOrWhiteSpace(registerUserDto.Password))
            {
                return BadRequest("Name and Password are required");
            }

            if (_context.Users.Any(u => u.Name == registerUserDto.Name))
            {
                return Conflict("User already exists");
            }

            var user = new User 
            { 
                Name = registerUserDto.Name,
                Password = registerUserDto.Password,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User is registered successfully");
           // return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
    }
}
