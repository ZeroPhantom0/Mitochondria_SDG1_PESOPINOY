using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public interface IInsuranceRepository : IRepository<InsurancePolicy>
    {
        Task<IEnumerable<InsurancePolicy>> GetActivePoliciesAsync();
        Task<IEnumerable<InsurancePolicy>> GetPoliciesByBorrowerIdAsync(int borrowerId);
        Task<InsurancePolicy?> GetPolicyWithClaimsAsync(int policyId);
        Task<IEnumerable<InsuranceClaim>> GetClaimsByPolicyIdAsync(int policyId);
        Task<InsuranceClaim?> GetClaimWithDetailsAsync(int claimId);
    }
}