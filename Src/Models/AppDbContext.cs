using Microsoft.EntityFrameworkCore;

namespace BankLoansApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Deposit> Deposits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "AQAAAAIAAYagAAAAEHHMuR8qXU3npK8frmQ6t3jgz8e31P9X7nYlH5o5U9rKk6N7m8LZVQ4s1vW2jHw==",
                FullName = "Администратор Системы",
                Role = "Admin"
            }
        );
    }
}
