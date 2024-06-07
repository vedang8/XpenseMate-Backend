using ExpenseTracker.Data;
using ExpenseTracker.Models.DTO;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ExpenseController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/expenses
        [Authorize]
        [HttpGet("expenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);
                var today = DateTime.Now.Date;
                var expenses = await _context.Expenses
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId && e.Date >= today && e.Date < today.AddDays(1))
                    .ToListAsync();

                return Ok(expenses);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/expenses/add
        [Authorize]
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Expense>> AddExpense(AddExpenseDto addExpenseDto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);
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

                return Ok(expense);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not logged in");
            }

            int userId = int.Parse(userIdClaim.Value);

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            // Check if the expense belongs to the current user
            if (expense.UserId != userId)
            {
                return Forbid(); // Return 403 Forbidden if the user is not the owner
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok("Expense deleted successfully");
        }

        [Authorize]
        [HttpGet("chart")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetTotalExpesesForChart()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);

                var totalExpenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.CategoryId)
                .Select(g => new CategoryExpenseDto
                {
                    CategoryName = g.FirstOrDefault().Category.Name,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .ToListAsync();

                return Ok(totalExpenses);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("current-month/total")]
        public async Task<ActionResult<decimal>> GetTotalCurrentMonthExpense()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);

                var currentDate = DateTime.Now.Date;
                var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
                var total = await _context.Expenses
                    .Where(e => e.UserId == userId && e.Date >= startOfMonth && e.Date <= endOfMonth)
                    .SumAsync(e => e.Amount);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("today/total")]
        public async Task<ActionResult<decimal>> GetTotalTodayExpense()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);

                var today = DateTime.Now.Date;
                
                var total = await _context.Expenses
                    .Where(e => e.UserId == userId && e.Date >= today && e.Date < today.AddDays(1))
                    .SumAsync(e => e.Amount);

                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("maximum")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetMaxExpense()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);

                var maxExpense = await _context.Expenses
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.Amount)
                    .FirstOrDefaultAsync();

                return Ok(maxExpense);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("minimum")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetMinExpense()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);

                var minimumExpense = await _context.Expenses
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId)
                    .OrderBy(e => e.Amount)
                    .FirstOrDefaultAsync();
                
                return Ok(minimumExpense);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Expense>>> FilterExpenses(
            [FromQuery] int? category,
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not logged in");
                }

                int userId = int.Parse(userIdClaim.Value);

                var query = _context.Expenses
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId)
                    .AsQueryable();

                if (category.HasValue)
                {
                    query = query.Where(e => e.CategoryId == category.Value);
                }

                if (year.HasValue)
                {
                    query = query.Where(e => e.Date.Year == year.Value);
                }

                if (month.HasValue)
                {
                    query = query.Where(e => e.Date.Month == month.Value);
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    query = query.Where(e => e.Date >= startDate.Value && e.Date <= endDate.Value);
                }

                var filteredExpenses = await query.ToListAsync();

                return Ok(filteredExpenses);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
