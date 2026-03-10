using System.Collections.Generic;
using System.Threading.Tasks;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.BLL.Interfaces
{
    public interface ILoanService
    {
        Task<Loan> ApplyForLoanAsync(Loan loan);
        Task<Loan> GetLoanByIdAsync(int id);
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<IEnumerable<Loan>> GetLoansByBorrowerIdAsync(int borrowerId);
        Task<Payment> RecordPaymentAsync(Payment payment);
        Task<List<Loan>> GetOverdueLoansAsync();
        Task<Loan> ApproveLoanAsync(int loanId);
        Task<Loan> RejectLoanAsync(int loanId, string reason);
        Task<decimal> CalculateMonthlyPayment(decimal principal, decimal interestRate, int term);
    }
}