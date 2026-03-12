using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.BLL.Services
{
    public class BackupService
    {
        private readonly AppDbContext _context;

        public BackupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task BackupDataAsync(string filePath, Action<string> logCallback)
        {
            try
            {
                logCallback("Starting database backup...");

                var backupData = new BackupData
                {
                    Users = await _context.Users.ToListAsync(),
                    Borrowers = await _context.Borrowers
                        .Include(b => b.Loans)
                        .Include(b => b.SavingsAccounts)
                        .Include(b => b.InsurancePolicies)
                        .ToListAsync(),
                    Loans = await _context.Loans
                        .Include(l => l.Payments)
                        .Include(l => l.AmortizationSchedules)
                        .ToListAsync(),
                    Payments = await _context.Payments.ToListAsync(),
                    AmortizationSchedules = await _context.AmortizationSchedules.ToListAsync(),
                    SavingsAccounts = await _context.SavingsAccounts
                        .Include(s => s.Transactions)
                        .ToListAsync(),
                    SavingsTransactions = await _context.SavingsTransactions.ToListAsync(),
                    InsurancePolicies = await _context.InsurancePolicies
                        .Include(i => i.Claims)
                        .Include(i => i.Payments)
                        .ToListAsync(),
                    InsuranceClaims = await _context.InsuranceClaims.ToListAsync(),
                    InsurancePayments = await _context.InsurancePayments.ToListAsync(),
                    AuditLogs = await _context.AuditLogs.ToListAsync()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };

                string jsonString = JsonSerializer.Serialize(backupData, options);
                await File.WriteAllTextAsync(filePath, jsonString);

                logCallback($"Backup completed successfully. Total records: {GetTotalRecords(backupData)}");
            }
            catch (Exception ex)
            {
                logCallback($"Backup failed: {ex.Message}");
                throw;
            }
        }

        public async Task RestoreDataAsync(string filePath, Action<string> logCallback)
        {
            try
            {
                logCallback($"Starting restore from: {filePath}");

                string jsonString = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };

                var backupData = JsonSerializer.Deserialize<BackupData>(jsonString, options);

                if (backupData == null)
                    throw new Exception("Invalid backup file format");

                logCallback("Clearing existing data...");
                await ClearExistingDataAsync(logCallback);

                // IMPORTANT: Clear all tracked entities after clearing data
                _context.ChangeTracker.Clear();
                logCallback("Cleared entity tracker");

                logCallback("Restoring data...");

                using (var dbTransaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Restore Users - with duplicate checking
                        if (backupData.Users?.Any() == true)
                        {
                            var uniqueUsers = backupData.Users
                                .GroupBy(u => u.Username)
                                .Select(g => g.First())
                                .ToList();

                            foreach (var user in uniqueUsers)
                            {
                                user.UserId = 0; // Reset ID
                                                 // Check if user with same username already exists in context
                                var existing = await _context.Users
                                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                                if (existing == null)
                                {
                                    await _context.Users.AddAsync(user);
                                }
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {uniqueUsers.Count} users");
                        }

                        // 2. Restore Borrowers
                        if (backupData.Borrowers?.Any() == true)
                        {
                            foreach (var borrower in backupData.Borrowers)
                            {
                                borrower.BorrowerId = 0;
                                borrower.Loans = null;
                                borrower.SavingsAccounts = null;
                                borrower.InsurancePolicies = null;
                                await _context.Borrowers.AddAsync(borrower);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.Borrowers.Count} borrowers");
                        }

                        // 3. Restore InsurancePolicies
                        if (backupData.InsurancePolicies?.Any() == true)
                        {
                            foreach (var policy in backupData.InsurancePolicies)
                            {
                                policy.InsurancePolicyId = 0;
                                policy.Claims = null;
                                policy.Payments = null;
                                policy.Loans = null;
                                await _context.InsurancePolicies.AddAsync(policy);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.InsurancePolicies.Count} insurance policies");
                        }

                        // 4. Restore SavingsAccounts
                        if (backupData.SavingsAccounts?.Any() == true)
                        {
                            foreach (var account in backupData.SavingsAccounts)
                            {
                                account.SavingsAccountId = 0;
                                account.Transactions = null;
                                await _context.SavingsAccounts.AddAsync(account);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.SavingsAccounts.Count} savings accounts");
                        }

                        // 5. Restore Loans
                        if (backupData.Loans?.Any() == true)
                        {
                            foreach (var loan in backupData.Loans)
                            {
                                loan.LoanId = 0;
                                loan.Payments = null;
                                loan.AmortizationSchedules = null;
                                await _context.Loans.AddAsync(loan);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.Loans.Count} loans");
                        }

                        // 6. Restore AmortizationSchedules
                        if (backupData.AmortizationSchedules?.Any() == true)
                        {
                            foreach (var schedule in backupData.AmortizationSchedules)
                            {
                                schedule.AmortizationScheduleId = 0;
                                await _context.AmortizationSchedules.AddAsync(schedule);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.AmortizationSchedules.Count} amortization schedules");
                        }

                        // 7. Restore Payments
                        if (backupData.Payments?.Any() == true)
                        {
                            foreach (var payment in backupData.Payments)
                            {
                                payment.PaymentId = 0;
                                await _context.Payments.AddAsync(payment);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.Payments.Count} payments");
                        }

                        // 8. Restore InsuranceClaims
                        if (backupData.InsuranceClaims?.Any() == true)
                        {
                            foreach (var claim in backupData.InsuranceClaims)
                            {
                                claim.InsuranceClaimId = 0;
                                await _context.InsuranceClaims.AddAsync(claim);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.InsuranceClaims.Count} insurance claims");
                        }

                        // 9. Restore InsurancePayments
                        if (backupData.InsurancePayments?.Any() == true)
                        {
                            foreach (var payment in backupData.InsurancePayments)
                            {
                                payment.InsurancePaymentId = 0;
                                await _context.InsurancePayments.AddAsync(payment);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.InsurancePayments.Count} insurance payments");
                        }

                        // 10. Restore SavingsTransactions
                        if (backupData.SavingsTransactions?.Any() == true)
                        {
                            foreach (var transaction in backupData.SavingsTransactions)
                            {
                                transaction.SavingsTransactionId = 0;
                                await _context.SavingsTransactions.AddAsync(transaction);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.SavingsTransactions.Count} savings transactions");
                        }

                        // 11. Restore AuditLogs
                        if (backupData.AuditLogs?.Any() == true)
                        {
                            foreach (var log in backupData.AuditLogs)
                            {
                                log.AuditLogId = 0;
                                await _context.AuditLogs.AddAsync(log);
                            }
                            await _context.SaveChangesAsync();
                            logCallback($"Restored {backupData.AuditLogs.Count} audit logs");
                        }

                        await dbTransaction.CommitAsync();
                        logCallback("Restore completed successfully!");
                    }
                    catch (Exception ex)
                    {
                        await dbTransaction.RollbackAsync();
                        logCallback($"ERROR during restore: {ex.Message}");
                        if (ex.InnerException != null)
                            logCallback($"Inner error: {ex.InnerException.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                logCallback($"Restore failed: {ex.Message}");
                throw;
            }
        }
        private async Task ClearExistingDataAsync(Action<string> logCallback)
        {
            try
            {
                // Disable foreign key checks temporarily (SQLite specific)
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys=OFF;");

                // Clear all tables using raw SQL
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM SavingsTransactions;");
                logCallback("Cleared savings transactions");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM InsurancePayments;");
                logCallback("Cleared insurance payments");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM InsuranceClaims;");
                logCallback("Cleared insurance claims");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Payments;");
                logCallback("Cleared payments");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM AmortizationSchedules;");
                logCallback("Cleared amortization schedules");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Loans;");
                logCallback("Cleared loans");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM SavingsAccounts;");
                logCallback("Cleared savings accounts");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM InsurancePolicies;");
                logCallback("Cleared insurance policies");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Borrowers;");
                logCallback("Cleared borrowers");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Users;");
                logCallback("Cleared users");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM AuditLogs;");
                logCallback("Cleared audit logs");

                // Reset auto-increment
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence;");

                // Re-enable foreign key checks
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys=ON;");

                logCallback("All data cleared successfully");
            }
            catch (Exception ex)
            {
                logCallback($"Error clearing data: {ex.Message}");
                // Re-enable foreign key checks even if error occurs
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys=ON;");
                throw;
            }
        }

        private int GetTotalRecords(BackupData data)
        {
            return (data.Users?.Count ?? 0) +
                   (data.Borrowers?.Count ?? 0) +
                   (data.Loans?.Count ?? 0) +
                   (data.Payments?.Count ?? 0) +
                   (data.AmortizationSchedules?.Count ?? 0) +
                   (data.SavingsAccounts?.Count ?? 0) +
                   (data.SavingsTransactions?.Count ?? 0) +
                   (data.InsurancePolicies?.Count ?? 0) +
                   (data.InsuranceClaims?.Count ?? 0) +
                   (data.InsurancePayments?.Count ?? 0) +
                   (data.AuditLogs?.Count ?? 0);
        }
    }

    public class BackupData
    {
        public List<User>? Users { get; set; }
        public List<Borrower>? Borrowers { get; set; }
        public List<Loan>? Loans { get; set; }
        public List<Payment>? Payments { get; set; }
        public List<AmortizationSchedule>? AmortizationSchedules { get; set; }
        public List<SavingsAccount>? SavingsAccounts { get; set; }
        public List<SavingsTransaction>? SavingsTransactions { get; set; }
        public List<InsurancePolicy>? InsurancePolicies { get; set; }
        public List<InsuranceClaim>? InsuranceClaims { get; set; }
        public List<InsurancePayment>? InsurancePayments { get; set; }
        public List<AuditLog>? AuditLogs { get; set; }
    }
}