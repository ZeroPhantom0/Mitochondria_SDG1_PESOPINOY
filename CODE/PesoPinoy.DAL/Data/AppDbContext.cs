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

            // =====================================================
            // NEW RELATIONSHIP CONFIGURATIONS
            // =====================================================

            // Configure InsurancePolicy - Loan relationship
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.InsurancePolicy)
                .WithMany(i => i.Loans)
                .HasForeignKey(l => l.InsurancePolicyId)
                .OnDelete(DeleteBehavior.SetNull); // When insurance policy is deleted, set Loan's InsurancePolicyId to NULL

            // Configure InsurancePolicy - InsurancePayment relationship
            modelBuilder.Entity<InsurancePayment>()
                .HasOne(ip => ip.InsurancePolicy)
                .WithMany(i => i.Payments)
                .HasForeignKey(ip => ip.InsurancePolicyId)
                .OnDelete(DeleteBehavior.Cascade); // When policy is deleted, delete all its payments

            // Configure InsurancePolicy - InsuranceClaim relationship
            modelBuilder.Entity<InsuranceClaim>()
                .HasOne(ic => ic.InsurancePolicy)
                .WithMany(i => i.Claims)
                .HasForeignKey(ic => ic.InsurancePolicyId)
                .OnDelete(DeleteBehavior.Cascade); // When policy is deleted, delete all its claims

            // Configure SavingsAccount - SavingsTransaction relationship
            modelBuilder.Entity<SavingsTransaction>()
                .HasOne(st => st.SavingsAccount)
                .WithMany(sa => sa.Transactions)
                .HasForeignKey(st => st.SavingsAccountId)
                .OnDelete(DeleteBehavior.Cascade); // When account is deleted, delete all its transactions

            // Configure Loan - Payment relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Loan)
                .WithMany(l => l.Payments)
                .HasForeignKey(p => p.LoanId)
                .OnDelete(DeleteBehavior.Cascade); // When loan is deleted, delete all its payments

            // Configure Loan - AmortizationSchedule relationship
            modelBuilder.Entity<AmortizationSchedule>()
                .HasOne(a => a.Loan)
                .WithMany(l => l.AmortizationSchedules)
                .HasForeignKey(a => a.LoanId)
                .OnDelete(DeleteBehavior.Cascade); // When loan is deleted, delete all its schedules

            // Configure Borrower - Loan relationship
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Borrower)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting borrower with active loans

            // Configure Borrower - SavingsAccount relationship
            modelBuilder.Entity<SavingsAccount>()
                .HasOne(sa => sa.Borrower)
                .WithMany(b => b.SavingsAccounts)
                .HasForeignKey(sa => sa.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting borrower with savings accounts

            // Configure Borrower - InsurancePolicy relationship
            modelBuilder.Entity<InsurancePolicy>()
                .HasOne(ip => ip.Borrower)
                .WithMany(b => b.InsurancePolicies)
                .HasForeignKey(ip => ip.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting borrower with insurance policies

            // Configure AuditLog - User relationship (optional)
            modelBuilder.Entity<AuditLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.SetNull); // When user is deleted, set UserId to NULL in audit logs

            // Configure decimal precision for all decimal properties
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var decimalProperties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    property.SetPrecision(18);
                    property.SetScale(2);
                }
            }
        }
    }
}