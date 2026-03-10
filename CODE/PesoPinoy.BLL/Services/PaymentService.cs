using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;
using PesoPinoy.BLL.Helpers;

namespace PesoPinoy.BLL.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Borrower)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByLoanIdAsync(int loanId)
        {
            return await _context.Payments
                .Where(p => p.LoanId == loanId)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> RecordPaymentAsync(Payment payment)
        {
            var loan = await _context.Loans
                .Include(l => l.AmortizationSchedules)
                .FirstOrDefaultAsync(l => l.LoanId == payment.LoanId);

            if (loan == null)
                throw new Exception("Loan not found");

            // Calculate late penalty if applicable
            if (payment.PaymentDate > payment.DueDate)
            {
                payment.DaysLate = (payment.PaymentDate - payment.DueDate).Days;
                payment.LatePenalty = PenaltyCalculator.CalculatePenalty(
                    payment.AmountDue,
                    payment.DaysLate
                );
            }

            // Update the corresponding amortization schedule
            var schedule = loan.AmortizationSchedules
                .FirstOrDefault(s => s.DueDate.Date == payment.DueDate.Date);

            if (schedule != null)
            {
                if (payment.AmountPaid >= payment.AmountDue)
                {
                    schedule.Status = PaymentStatus.Paid;
                }
                else if (payment.AmountPaid > 0)
                {
                    schedule.Status = PaymentStatus.Partial;
                }
                schedule.PaidDate = payment.PaymentDate;
            }

            // Update loan balance
            loan.BalanceRemaining -= payment.AmountPaid;

            if (loan.BalanceRemaining <= 0)
            {
                loan.Status = LoanStatus.Completed;
                loan.BalanceRemaining = 0;
            }

            payment.PaymentNumber = GeneratePaymentNumber();
            payment.Status = DeterminePaymentStatus(payment);

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        private PaymentStatus DeterminePaymentStatus(Payment payment)
        {
            if (payment.AmountPaid >= payment.AmountDue)
                return PaymentStatus.Paid;
            else if (payment.AmountPaid > 0)
                return PaymentStatus.Partial;
            else
                return PaymentStatus.Pending;
        }

        public async Task<decimal> GetTotalCollectionsAsync(DateTime startDate, DateTime endDate)
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
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Loan)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        private string GeneratePaymentNumber()
        {
            return $"PAY-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}