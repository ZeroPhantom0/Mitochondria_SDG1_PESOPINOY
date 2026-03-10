using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public class InsuranceRepository : Repository<InsurancePolicy>, IInsuranceRepository
    {
        public InsuranceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InsurancePolicy>> GetActivePoliciesAsync()
        {
            return await _context.InsurancePolicies
                .Include(i => i.Borrower)
                .Where(i => i.IsActive && i.EndDate >= DateTime.Now)
                .ToListAsync();
        }

        public async Task<IEnumerable<InsurancePolicy>> GetPoliciesByBorrowerIdAsync(int borrowerId)
        {
            return await _context.InsurancePolicies
                .Include(i => i.Claims)
                .Where(i => i.BorrowerId == borrowerId)
                .ToListAsync();
        }

        public async Task<InsurancePolicy?> GetPolicyWithClaimsAsync(int policyId)
        {
            return await _context.InsurancePolicies
                .Include(i => i.Borrower)
                .Include(i => i.Claims)
                .FirstOrDefaultAsync(i => i.InsurancePolicyId == policyId);
        }

        public async Task<IEnumerable<InsuranceClaim>> GetClaimsByPolicyIdAsync(int policyId)
        {
            return await _context.InsuranceClaims
                .Where(c => c.InsurancePolicyId == policyId)
                .OrderByDescending(c => c.FilingDate)
                .ToListAsync();
        }

        public async Task<InsuranceClaim?> GetClaimWithDetailsAsync(int claimId)
        {
            return await _context.InsuranceClaims
                .Include(c => c.InsurancePolicy)
                .ThenInclude(p => p.Borrower)
                .FirstOrDefaultAsync(c => c.InsuranceClaimId == claimId);
        }
    }
}