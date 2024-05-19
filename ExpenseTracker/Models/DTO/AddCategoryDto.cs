using ExpenseTracker.Models.Entities;

namespace ExpenseTracker.Models.DTO
{
    public class AddCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public List<Expense> Expenses { get; set; }
    }
}
