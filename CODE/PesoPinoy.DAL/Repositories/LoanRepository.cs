using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.DAL.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Loan>> GetLoansWithDetailsAsync()
        {
            return await _context.Loans
                .Include(l => l.Borrower)
                .Include(l => l.Payments)
                .Include(l => l.AmortizationSchedules)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Borrower)
                .Where(l => l.Status == LoanStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansByBorrowerIdAsync(int borrowerId)
        {
            return await _context.Loans
                .Include(l => l.Payments)
                .Where(l => l.BorrowerId == borrowerId)
                .ToListAsync();
        }

        public async Task<Loan?> GetLoanWithPaymentsAsync(int loanId)
        {
            return await _context.Loans
                .Include(l => l.Borrower)
                .Include(l => l.Payments)
                .Include(l => l.AmortizationSchedules)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);
        }

        public async Task<decimal> GetTotalOutstandingAsync()
        {
            return await _context.Loans
                .Where(l => l.Status == LoanStatus.Active)
                .SumAsync(l => l.BalanceRemaining);
        }
    }
}