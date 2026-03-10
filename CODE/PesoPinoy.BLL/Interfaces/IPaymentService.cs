using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> RecordPaymentAsync(Payment payment);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<IEnumerable<Payment>> GetPaymentsByLoanIdAsync(int loanId);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalCollectionsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetOverduePaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(int id);
    }
}