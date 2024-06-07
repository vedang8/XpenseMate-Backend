using System.Text.Json.Serialization;

namespace ExpenseTracker.Models.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public string Name { get; set; }    

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date {  get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
