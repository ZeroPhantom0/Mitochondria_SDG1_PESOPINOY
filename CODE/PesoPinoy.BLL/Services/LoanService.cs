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
    public class LoanService
    {
        private readonly AppDbContext _context;
        private readonly RiskAnalysisService _riskService;

        public LoanService(AppDbContext context, RiskAnalysisService riskService)
        {
            _context = context;
            _riskService = riskService;
        }

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Borrower)
                .Include(l => l.Payments)
                .Include(l => l.AmortizationSchedules)
                .OrderByDescending(l => l.ApplicationDate)
                .ToListAsync();
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Borrower)
                .Include(l => l.Payments)
                .Include(l => l.AmortizationSchedules)
                .FirstOrDefaultAsync(l => l.LoanId == id);
        }

        public async Task<IEnumerable<Loan>> GetLoansByBorrowerIdAsync(int borrowerId)
        {
            return await _context.Loans
                .Where(l => l.BorrowerId == borrowerId)
                .ToListAsync();
        }

        public async Task<Loan> ApplyForLoanAsync(Loan loan)
        {
            // Generate loan number
            loan.LoanNumber = GenerateLoanNumber();

            // Get borrower risk score
            var borrower = await _context.Borrowers
                .FindAsync(loan.BorrowerId);

            if (borrower == null)
                throw new Exception("Borrower not found");

            // Calculate loan details
            var loanCalculator = new LoanCalculator();
            var result = loanCalculator.CalculateLoan(
                loan.PrincipalAmount,
                loan.InterestRate,
                loan.TermInMonths
            );

            loan.MonthlyPayment = result.MonthlyPayment;
            loan.TotalInterest = result.TotalInterest;
            loan.TotalAmountPayable = result.TotalAmount;
            loan.BalanceRemaining = result.TotalAmount;
            loan.ApplicationDate = DateTime.Now;
            loan.Status = LoanStatus.Pending;

            // Generate amortization schedule
            var schedule = loanCalculator.GenerateAmortizationSchedule(
                loan.PrincipalAmount,
                loan.InterestRate,
                loan.TermInMonths,
                loan.FirstPaymentDate
            );

            loan.AmortizationSchedules = schedule.Select(s => new AmortizationSchedule
            {
                PaymentNumber = s.PaymentNumber,
                DueDate = s.DueDate,
                BeginningBalance = s.BeginningBalance,
                PaymentAmount = s.PaymentAmount,
                PrincipalAmount = s.PrincipalAmount,
                InterestAmount = s.InterestAmount,
                EndingBalance = s.EndingBalance,
                Status = PaymentStatus.Pending
            }).ToList();

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<Loan> ApproveLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
                throw new Exception("Loan not found");

            loan.Status = LoanStatus.Active;
            loan.ApprovalDate = DateTime.Now;
            loan.DisbursementDate = DateTime.Now;
            loan.MaturityDate = loan.FirstPaymentDate.AddMonths(loan.TermInMonths);

            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<Loan> RejectLoanAsync(int loanId, string reason)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
                throw new Exception("Loan not found");

            loan.Status = LoanStatus.Rejected;
            await _context.SaveChangesAsync();

            return loan;
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
                schedule.Status = payment.AmountPaid >= payment.AmountDue ?
                    PaymentStatus.Paid : PaymentStatus.Partial;
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
            payment.Status = payment.AmountPaid >= payment.AmountDue ?
                PaymentStatus.Paid : PaymentStatus.Partial;

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<List<Loan>> GetOverdueLoansAsync()
        {
            var today = DateTime.Now;

            var overdueLoans = await _context.Loans
                .Include(l => l.Borrower)
                .Include(l => l.AmortizationSchedules)
                .Where(l => l.Status == LoanStatus.Active &&
                       l.AmortizationSchedules.Any(s =>
                           s.Status == PaymentStatus.Pending &&
                           s.DueDate < today))
                .ToListAsync();

            return overdueLoans;
        }

        private string GenerateLoanNumber()
        {
            return $"LN-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private string GeneratePaymentNumber()
        {
            return $"PAY-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}