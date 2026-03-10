using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public interface ISavingsRepository : IRepository<SavingsAccount>
    {
        Task<SavingsAccount?> GetAccountByBorrowerIdAsync(int borrowerId);
        Task<IEnumerable<SavingsTransaction>> GetTransactionsByAccountIdAsync(int accountId);
        Task<SavingsAccount?> GetAccountWithTransactionsAsync(int accountId);
        Task<decimal> GetTotalSavingsAsync();
    }
}