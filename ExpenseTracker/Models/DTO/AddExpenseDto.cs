namespace ExpenseTracker.Models.DTO
{
    public class AddExpenseDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
    }
}
