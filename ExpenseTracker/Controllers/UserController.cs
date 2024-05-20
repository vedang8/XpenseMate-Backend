using ExpenseTracker.Data;
using ExpenseTracker.Models.DTO;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpPost("login")]
        public ActionResult<User> Login(LoginUserDto loginUserDto)
        {
            if (string.IsNullOrWhiteSpace(loginUserDto.Name) || string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                return BadRequest("Please provide proper details");
            }

            var user = _context.Users.SingleOrDefault(u => u.Name == loginUserDto.Name);
            if ((user == null) || (loginUserDto.Password != user.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // create session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            return Ok("User logged in successfully");

        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("User logged out successfully");
        }
    }
}
