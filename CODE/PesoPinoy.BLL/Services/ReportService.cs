using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.BLL.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetMonthlyCollectionsReportAsync(int year)
        {
            var monthlyData = new List<object>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var collections = await _context.Payments
                    .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                    .SumAsync(p => p.AmountPaid);

                monthlyData.Add(new { Month = startDate.ToString("MMM"), Amount = collections });
            }

            return monthlyData;
        }

        public async Task<object> GetLoanPerformanceReportAsync()
        {
            var totalLoans = await _context.Loans.CountAsync();
            var activeLoans = await _context.Loans.CountAsync(l => l.Status == Models.Enums.LoanStatus.Active);
            var completedLoans = await _context.Loans.CountAsync(l => l.Status == Models.Enums.LoanStatus.Completed);
            var defaultedLoans = await _context.Loans.CountAsync(l => l.Status == Models.Enums.LoanStatus.Defaulted);

            return new
            {
                totalLoans,
                activeLoans,
                completedLoans,
                defaultedLoans,
                performanceRate = totalLoans > 0 ? (completedLoans / (double)totalLoans) * 100 : 0
            };
        }

        public async Task<object> GetBorrowerDemographicsReportAsync()
        {
            var employmentStats = await _context.Borrowers
                .GroupBy(b => b.EmploymentStatus)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var incomeBrackets = new[]
            {
                new { Bracket = "Below 10k", Min = 0m, Max = 10000m },
                new { Bracket = "10k - 20k", Min = 10000m, Max = 20000m },
                new { Bracket = "20k - 30k", Min = 20000m, Max = 30000m },
                new { Bracket = "30k - 50k", Min = 30000m, Max = 50000m },
                new { Bracket = "Above 50k", Min = 50000m, Max = decimal.MaxValue }
            };

            var incomeStats = new List<object>();
            foreach (var bracket in incomeBrackets)
            {
                var count = await _context.Borrowers
                    .CountAsync(b => b.MonthlyIncome >= bracket.Min && b.MonthlyIncome < bracket.Max);
                incomeStats.Add(new { bracket.Bracket, Count = count });
            }

            return new
            {
                employmentStats,
                incomeStats
            };
        }
    }
}