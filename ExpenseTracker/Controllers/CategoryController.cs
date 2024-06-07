using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
        }

        // GET: api/expenses
        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetCategories()
        {
            var expenses = await _context.Categories.ToListAsync();
            return Ok(expenses);
        }
    }
}
