using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PesoPinoy.Models.Entities;
using System.IO;

namespace PesoPinoy.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }
        public DbSet<SavingsTransaction> SavingsTransactions { get; set; }
        public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
        public DbSet<AmortizationSchedule> AmortizationSchedules { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<InsurancePayment> InsurancePayments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Get the base directory
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                // Go up to the project root
                var projectRoot = Path.GetFullPath(Path.Combine(basePath, "..\\..\\..\\..\\..\\"));
                var dbPath = Path.Combine(projectRoot, "INPUT_DATA", "pesopinoy.db");

                // Ensure directory exists
                var directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Borrower>()
                .HasIndex(b => b.Email)
                .IsUnique();

            modelBuilder.Entity<Loan>()
                .HasIndex(l => l.LoanNumber)
                .IsUnique();

            modelBuilder.Entity<Loan>()
                .Property(l => l.InterestRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Loan>()
                .Property(l => l.PrincipalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PaymentNumber)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .Property(p => p.AmountPaid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SavingsAccount>()
                .HasIndex(s => s.AccountNumber)
                .IsUnique();

            modelBuilder.Entity<InsurancePolicy>()
                .HasIndex(i => i.PolicyNumber)
                .IsUnique();
        }
    }
}