using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;
using PesoPinoy.BLL.Helpers;

namespace PesoPinoy.BLL.Services
{
    public class BorrowerService
    {
        private readonly AppDbContext _context;

        public BorrowerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Borrower>> GetAllBorrowersAsync()
        {
            return await _context.Borrowers
                .Include(b => b.Loans)
                .Include(b => b.SavingsAccounts)
                .Include(b => b.InsurancePolicies)
                .ToListAsync();
        }

        public async Task<Borrower> GetBorrowerByIdAsync(int id)
        {
            return await _context.Borrowers
                .Include(b => b.Loans)
                .Include(b => b.SavingsAccounts)
                .Include(b => b.InsurancePolicies)
                .FirstOrDefaultAsync(b => b.BorrowerId == id);
        }

        public async Task<Borrower> AddBorrowerAsync(Borrower borrower)
        {
            // Calculate risk score
            borrower.RiskScore = RiskScoreCalculator.CalculateRiskScore(borrower);
            borrower.RiskClassification = RiskScoreCalculator.ClassifyRisk(borrower.RiskScore);

            await _context.Borrowers.AddAsync(borrower);
            await _context.SaveChangesAsync();

            return borrower;
        }

        public async Task<Borrower> UpdateBorrowerAsync(Borrower borrower)
        {
            var existingBorrower = await _context.Borrowers.FindAsync(borrower.BorrowerId);
            if (existingBorrower == null)
                throw new Exception("Borrower not found");

            // Update properties
            existingBorrower.FirstName = borrower.FirstName;
            existingBorrower.LastName = borrower.LastName;
            existingBorrower.MiddleName = borrower.MiddleName;
            existingBorrower.DateOfBirth = borrower.DateOfBirth;
            existingBorrower.ContactNumber = borrower.ContactNumber;
            existingBorrower.Email = borrower.Email;
            existingBorrower.Address = borrower.Address;
            existingBorrower.EmploymentStatus = borrower.EmploymentStatus;
            existingBorrower.MonthlyIncome = borrower.MonthlyIncome;
            existingBorrower.EmployerName = borrower.EmployerName;
            existingBorrower.GuarantorName = borrower.GuarantorName;
            existingBorrower.GuarantorContact = borrower.GuarantorContact;
            existingBorrower.ReasonForLoan = borrower.ReasonForLoan;

            // Recalculate risk score
            existingBorrower.RiskScore = RiskScoreCalculator.CalculateRiskScore(existingBorrower);
            existingBorrower.RiskClassification = RiskScoreCalculator.ClassifyRisk(existingBorrower.RiskScore);

            await _context.SaveChangesAsync();

            return existingBorrower;
        }

        public async Task DeleteBorrowerAsync(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
                throw new Exception("Borrower not found");

            // Check if borrower has active loans
            var hasActiveLoans = await _context.Loans
                .AnyAsync(l => l.BorrowerId == id && l.Status == LoanStatus.Active);

            if (hasActiveLoans)
                throw new Exception("Cannot delete borrower with active loans");

            _context.Borrowers.Remove(borrower);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Borrower>> GetHighRiskBorrowersAsync()
        {
            return await _context.Borrowers
                .Where(b => b.RiskClassification == RiskClassification.High ||
                           b.RiskClassification == RiskClassification.VeryHigh)
                .ToListAsync();
        }
    }
}
