using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public interface ILoanRepository : IRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetLoansWithDetailsAsync();
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetLoansByBorrowerIdAsync(int borrowerId);
        Task<Loan?> GetLoanWithPaymentsAsync(int loanId);
        Task<decimal> GetTotalOutstandingAsync();
    }
}