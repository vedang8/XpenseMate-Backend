using ExpenseTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed predefined categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food", Emoji = "🍔"},
                new Category { Id = 2, Name = "Personal Care" , Emoji = "😉"},
                new Category { Id = 3, Name = "Entertainment", Emoji = "🎬" },
                new Category { Id = 4, Name = "Shopping", Emoji = "🛍️"},
                new Category { Id = 5, Name = "Medical", Emoji = "🩺" },
                new Category { Id = 6, Name = "Transportation", Emoji = "🚋" },
                new Category { Id = 7, Name = "Rent", Emoji = "🏠" },
                new Category { Id = 8, Name = "Fuel", Emoji = "⛽" },
                new Category { Id = 9, Name = "Grocery", Emoji = "🫛" },
                new Category { Id = 10, Name = "Others", Emoji = "📦" }
            );

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); ;

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Expenses)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }       
    }
}
