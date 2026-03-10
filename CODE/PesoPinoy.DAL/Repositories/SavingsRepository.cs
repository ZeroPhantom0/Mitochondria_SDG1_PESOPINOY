using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public class SavingsRepository : Repository<SavingsAccount>, ISavingsRepository
    {
        public SavingsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<SavingsAccount?> GetAccountByBorrowerIdAsync(int borrowerId)
        {
            return await _context.SavingsAccounts
                .Include(s => s.Transactions)
                .FirstOrDefaultAsync(s => s.BorrowerId == borrowerId);
        }

        public async Task<IEnumerable<SavingsTransaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await _context.SavingsTransactions
                .Where(t => t.SavingsAccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<SavingsAccount?> GetAccountWithTransactionsAsync(int accountId)
        {
            return await _context.SavingsAccounts
                .Include(s => s.Borrower)
                .Include(s => s.Transactions)
                .FirstOrDefaultAsync(s => s.SavingsAccountId == accountId);
        }

        public async Task<decimal> GetTotalSavingsAsync()
        {
            return await _context.SavingsAccounts
                .Where(s => s.IsActive)
                .SumAsync(s => s.CurrentBalance);
        }
    }
}