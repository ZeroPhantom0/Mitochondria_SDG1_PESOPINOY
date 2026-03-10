using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByLoanIdAsync(int loanId);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalCollectionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetOverduePaymentsAsync();
        Task<Payment?> GetPaymentWithDetailsAsync(int paymentId);
    }
}