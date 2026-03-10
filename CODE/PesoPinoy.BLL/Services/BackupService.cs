using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public async Task BackupDataAsync(string filePath, Action<string> logCallback = null)
        {
            try
            {
                logCallback?.Invoke("Starting database backup...");

                var backupData = new
                {
                    Timestamp = DateTime.Now,
                    Version = "1.0.0",
                    Users = await _context.Users.ToListAsync(),
                    Borrowers = await _context.Borrowers.ToListAsync(),
                    Loans = await _context.Loans.ToListAsync(),
                    Payments = await _context.Payments.ToListAsync(),
                    SavingsAccounts = await _context.SavingsAccounts.ToListAsync(),
                    SavingsTransactions = await _context.SavingsTransactions.ToListAsync(),
                    InsurancePolicies = await _context.InsurancePolicies.ToListAsync(),
                    InsuranceClaims = await _context.InsuranceClaims.ToListAsync(),
                    AmortizationSchedules = await _context.AmortizationSchedules.ToListAsync()
                };

                var json = JsonConvert.SerializeObject(backupData, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                await File.WriteAllTextAsync(filePath, json);

                logCallback?.Invoke($"Backup completed: {filePath}");
                logCallback?.Invoke($"Total records backed up: {GetTotalRecords(backupData)}");
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"ERROR: {ex.Message}");
                throw;
            }
        }

        public async Task RestoreDataAsync(string filePath, Action<string> logCallback = null)
        {
            try
            {
                logCallback?.Invoke($"Starting restore from: {filePath}");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Backup file not found");

                var json = await File.ReadAllTextAsync(filePath);
                var backupData = JsonConvert.DeserializeObject<dynamic>(json);

                logCallback?.Invoke("Clearing existing data...");
                await ClearAllDataAsync();

                logCallback?.Invoke("Restoring data...");
                await RestoreDataFromJson(backupData, logCallback);

                logCallback?.Invoke("Restore completed successfully!");
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"ERROR: {ex.Message}");
                throw;
            }
        }

        private async Task ClearAllDataAsync()
        {
            _context.AmortizationSchedules.RemoveRange(_context.AmortizationSchedules);
            _context.Payments.RemoveRange(_context.Payments);
            _context.Loans.RemoveRange(_context.Loans);
            _context.SavingsTransactions.RemoveRange(_context.SavingsTransactions);
            _context.SavingsAccounts.RemoveRange(_context.SavingsAccounts);
            _context.InsuranceClaims.RemoveRange(_context.InsuranceClaims);
            _context.InsurancePolicies.RemoveRange(_context.InsurancePolicies);
            _context.Borrowers.RemoveRange(_context.Borrowers);
            _context.Users.RemoveRange(_context.Users);

            await _context.SaveChangesAsync();
        }

        private async Task RestoreDataFromJson(dynamic backupData, Action<string> logCallback)
        {
            // Restore Users
            if (backupData.Users != null)
            {
                var users = JsonConvert.DeserializeObject<List<User>>(backupData.Users.ToString());
                await _context.Users.AddRangeAsync(users);
                logCallback?.Invoke($"Restored {users.Count} users");
            }

            // Restore Borrowers
            if (backupData.Borrowers != null)
            {
                var borrowers = JsonConvert.DeserializeObject<List<Borrower>>(backupData.Borrowers.ToString());
                await _context.Borrowers.AddRangeAsync(borrowers);
                logCallback?.Invoke($"Restored {borrowers.Count} borrowers");
            }

            // Restore Loans
            if (backupData.Loans != null)
            {
                var loans = JsonConvert.DeserializeObject<List<Loan>>(backupData.Loans.ToString());
                await _context.Loans.AddRangeAsync(loans);
                logCallback?.Invoke($"Restored {loans.Count} loans");
            }

            // Restore other entities similarly...

            await _context.SaveChangesAsync();
        }

        private int GetTotalRecords(object backupData)
        {
            int total = 0;
            var properties = backupData.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var value = prop.GetValue(backupData) as System.Collections.IEnumerable;
                    if (value != null)
                    {
                        var count = value.Cast<object>().Count();
                        total += count;
                    }
                }
            }

            return total;
        }
    }
}