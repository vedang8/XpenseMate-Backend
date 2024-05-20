using ExpenseTracker.Data;
using ExpenseTracker.Models.DTO;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }
        // POST: api/expenses/add
        [HttpPost("add")]
        public async Task<ActionResult<Expense>> AddExpense(AddExpenseDto addExpenseDto)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return Unauthorized("User is not logged in");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var expense = new Expense 
            { 
                Name = addExpenseDto.Name,
                Description = addExpenseDto.Description,
                Amount = addExpenseDto.Amount,
                Date = addExpenseDto.Date,
                CategoryId = addExpenseDto.CategoryId,
                UserId = userId
            };

            return Ok("Expense added successfully");
        } 
    }
}
