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
    public class RiskAnalysisService
    {
        private readonly AppDbContext _context;

        public RiskAnalysisService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, object>> GetPortfolioRiskAnalysisAsync()
        {
            var totalBorrowers = await _context.Borrowers.CountAsync();
            var activeLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Active);
            var overdueLoans = await _context.Loans
                .CountAsync(l => l.Status == LoanStatus.Active &&
                    l.AmortizationSchedules.Any(s => s.Status == PaymentStatus.Pending && s.DueDate < DateTime.Now));

            var riskDistribution = await _context.Borrowers
                .GroupBy(b => b.RiskClassification)
                .Select(g => new { Risk = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Risk.ToString(), x => x.Count);

            var totalLoanAmount = await _context.Loans
                .Where(l => l.Status == LoanStatus.Active)
                .SumAsync(l => l.BalanceRemaining);

            var highRiskExposure = await _context.Loans
                .Where(l => l.Status == LoanStatus.Active &&
                    l.Borrower.RiskClassification == RiskClassification.High ||
                    l.Borrower.RiskClassification == RiskClassification.VeryHigh)
                .SumAsync(l => l.BalanceRemaining);

            return new Dictionary<string, object>
            {
                { "totalBorrowers", totalBorrowers },
                { "activeLoans", activeLoans },
                { "overdueLoans", overdueLoans },
                { "riskDistribution", riskDistribution },
                { "totalLoanAmount", totalLoanAmount },
                { "highRiskExposure", highRiskExposure },
                { "riskPercentage", totalLoanAmount > 0 ? (highRiskExposure / totalLoanAmount) * 100 : 0 }
            };
        }

        public async Task<List<Borrower>> GetBorrowersNeedingReviewAsync()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            return await _context.Borrowers
                .Include(b => b.Loans)
                .Where(b => b.RiskClassification == RiskClassification.High ||
                           b.RiskClassification == RiskClassification.VeryHigh ||
                           b.Loans.Any(l => l.Status == LoanStatus.Active &&
                               l.AmortizationSchedules.Any(s => s.Status == PaymentStatus.Overdue)))
                .ToListAsync();
        }
    }
}
