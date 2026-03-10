using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.DAL.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByLoanIdAsync(int loanId)
        {
            return await _context.Payments
                .Where(p => p.LoanId == loanId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Borrower)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCollectionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .SumAsync(p => p.AmountPaid);
        }

        public async Task<IEnumerable<Payment>> GetOverduePaymentsAsync()
        {
            var today = DateTime.Now;
            return await _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Borrower)
                .Where(p => p.DueDate < today && p.Status != PaymentStatus.Paid)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentWithDetailsAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Borrower)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }
    }
}