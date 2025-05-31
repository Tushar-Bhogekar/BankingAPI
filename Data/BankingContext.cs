using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Data
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options) { }

        // class is responsible for interacting with the database.
        public DbSet<User> Users { get; set; } //
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Move to the top

            // One-to-one relationship between User and Account
            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<Account>(a => a.UserId);

            // Precision for decimal fields
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            // Seed an initial admin for testing
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                Username = "Aditya",
                Password = "Adi45" // In production, use hashed passwords
            });
        }
    }
}
