using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.BLL.Services
{
    public class SavingsService
    {
        private readonly AppDbContext _context;

        public SavingsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SavingsAccount>> GetAllSavingsAccountsAsync()
        {
            return await _context.SavingsAccounts
                .Include(s => s.Borrower)
                .Include(s => s.Transactions)
                .ToListAsync();
        }

        public async Task<SavingsAccount> GetSavingsAccountByIdAsync(int id)
        {
            return await _context.SavingsAccounts
                .Include(s => s.Borrower)
                .Include(s => s.Transactions)
                .FirstOrDefaultAsync(s => s.SavingsAccountId == id);
        }

        public async Task<SavingsAccount> GetSavingsAccountByBorrowerIdAsync(int borrowerId)
        {
            return await _context.SavingsAccounts
                .Include(s => s.Transactions)
                .FirstOrDefaultAsync(s => s.BorrowerId == borrowerId);
        }

        public async Task<SavingsAccount> CreateSavingsAccountAsync(int borrowerId)
        {
            var existing = await GetSavingsAccountByBorrowerIdAsync(borrowerId);
            if (existing != null)
                throw new Exception("Borrower already has a savings account");

            var account = new SavingsAccount
            {
                BorrowerId = borrowerId,
                AccountNumber = GenerateAccountNumber(),
                CurrentBalance = 0,
                OpenedDate = DateTime.Now,
                LastTransactionDate = DateTime.Now,
                IsActive = true
            };

            await _context.SavingsAccounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return account;
        }

        private string GenerateAccountNumber()
        {
            return $"SAV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public async Task<SavingsTransaction> DepositAsync(int accountId, decimal amount, string description = "")
        {
            var account = await _context.SavingsAccounts.FindAsync(accountId);
            if (account == null)
                throw new Exception("Savings account not found");

            var transaction = new SavingsTransaction
            {
                SavingsAccountId = accountId,
                TransactionDate = DateTime.Now,
                TransactionType = "Deposit",
                Amount = amount,
                BalanceBefore = account.CurrentBalance,
                BalanceAfter = account.CurrentBalance + amount,
                Description = description,
                ReferenceNumber = GenerateTransactionNumber()
            };

            account.CurrentBalance += amount;
            account.LastTransactionDate = DateTime.Now;

            await _context.SavingsTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<SavingsTransaction> WithdrawAsync(int accountId, decimal amount, string description = "")
        {
            var account = await _context.SavingsAccounts.FindAsync(accountId);
            if (account == null)
                throw new Exception("Savings account not found");

            if (account.CurrentBalance < amount)
                throw new Exception("Insufficient balance");

            var transaction = new SavingsTransaction
            {
                SavingsAccountId = accountId,
                TransactionDate = DateTime.Now,
                TransactionType = "Withdrawal",
                Amount = -amount,
                BalanceBefore = account.CurrentBalance,
                BalanceAfter = account.CurrentBalance - amount,
                Description = description,
                ReferenceNumber = GenerateTransactionNumber()
            };

            account.CurrentBalance -= amount;
            account.LastTransactionDate = DateTime.Now;

            await _context.SavingsTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        private string GenerateTransactionNumber()
        {
            return $"TRX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public async Task<decimal> GetTotalSavingsAsync()
        {
            return await _context.SavingsAccounts.SumAsync(s => s.CurrentBalance);
        }
    }
}