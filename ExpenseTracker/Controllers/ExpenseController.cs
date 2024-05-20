using ExpenseTracker.Data;
using ExpenseTracker.Models.DTO;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .ToListAsync();
            return Ok(expenses);
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

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok("Expense added successfully");
        }

        // PUT: api/expenses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditExpense(int id, EditExpenseDto editExpenseDto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            expense.Name = editExpenseDto.Name;
            expense.Description = editExpenseDto.Description;
            expense.Amount = editExpenseDto.Amount;
            expense.Date = editExpenseDto.Date;
            expense.CategoryId = editExpenseDto.CategoryId;

            _context.Entry(expense).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Expense edited successfully");
        }

        // DELETE: api/expenses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok("Expense deleted successfully");
        }
    }
}
