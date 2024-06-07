using System.Text.Json.Serialization;

namespace ExpenseTracker.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Emoji { get; set; }

        public List<Expense> Expenses { get; set; }
    }
}
