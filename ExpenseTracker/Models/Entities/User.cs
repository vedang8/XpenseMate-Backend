namespace ExpenseTracker.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public List<Expense> Expenses { get; set; }
    }
}
