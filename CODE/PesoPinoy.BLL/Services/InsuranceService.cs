using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.BLL.Services
{
    public class InsuranceService
    {
        private readonly AppDbContext _context;

        public InsuranceService(AppDbContext context)
        {
            _context = context;
        }

        #region Policy Management

        public async Task<IEnumerable<InsurancePolicy>> GetAllPoliciesAsync()
        {
            return await _context.InsurancePolicies
                .Include(i => i.Borrower)
                .Include(i => i.Claims)
                .Include(i => i.Payments)
                .OrderByDescending(i => i.StartDate)
                .ToListAsync();
        }

        public async Task<InsurancePolicy> GetPolicyByIdAsync(int id)
        {
            return await _context.InsurancePolicies
                .Include(i => i.Borrower)
                .Include(i => i.Claims)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InsurancePolicyId == id);
        }

        public async Task<IEnumerable<InsurancePolicy>> GetPoliciesByBorrowerIdAsync(int borrowerId)
        {
            return await _context.InsurancePolicies
                .Where(i => i.BorrowerId == borrowerId)
                .Include(i => i.Payments)
                .ToListAsync();
        }

        public async Task<InsurancePolicy> CreatePolicyAsync(InsurancePolicy policy)
        {
            policy.PolicyNumber = GeneratePolicyNumber();
            policy.Status = InsurancePolicyStatus.Pending;
            policy.StatusRemarks = "Policy created, awaiting first payment";

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.InsurancePolicies.AddAsync(policy);
                await _context.SaveChangesAsync();

                // Generate payment schedule
                await GeneratePaymentSchedule(policy.InsurancePolicyId);

                await transaction.CommitAsync();
                return policy;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private string GeneratePolicyNumber()
        {
            var lastPolicy = _context.InsurancePolicies
                .OrderByDescending(p => p.InsurancePolicyId)
                .FirstOrDefault();

            int nextNumber = (lastPolicy?.InsurancePolicyId ?? 0) + 1;
            return $"INS-{DateTime.Now:yyyyMMdd}-{nextNumber:D4}";
        }

        public async Task UpdatePolicyStatus(int policyId)
        {
            var policy = await _context.InsurancePolicies
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.InsurancePolicyId == policyId);

            if (policy == null) return;

            // Check if policy has expired
            if (DateTime.Now > policy.EndDate)
            {
                policy.Status = InsurancePolicyStatus.Expired;
                policy.StatusRemarks = $"Policy expired on {policy.EndDate:d}";
            }
            else
            {
                var payments = policy.Payments.ToList();

                // Check for missed payments (overdue)
                var hasOverdue = payments.Any(p => p.PaymentStatus == "Pending" && p.DueDate < DateTime.Now);

                if (hasOverdue)
                {
                    policy.Status = InsurancePolicyStatus.Lapsed;
                    policy.StatusRemarks = "Policy lapsed due to missed payments";
                }
                else
                {
                    // Check if fully paid
                    var totalPaid = payments.Where(p => p.PaymentStatus == "Paid").Sum(p => p.Amount);
                    var totalPremium = payments.Sum(p => p.Amount);

                    if (totalPaid >= totalPremium)
                    {
                        policy.Status = InsurancePolicyStatus.PaidUp;
                        policy.StatusRemarks = "Policy fully paid";
                    }
                    else if (payments.Any(p => p.PaymentStatus == "Paid"))
                    {
                        policy.Status = InsurancePolicyStatus.Active;
                        policy.StatusRemarks = "Policy active - payments up to date";
                    }
                    else
                    {
                        policy.Status = InsurancePolicyStatus.Pending;
                        policy.StatusRemarks = "Awaiting first payment";
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Payment Management

        public async Task GeneratePaymentSchedule(int policyId)
        {
            var policy = await _context.InsurancePolicies.FindAsync(policyId);
            if (policy == null) return;

            var payments = new List<InsurancePayment>();
            var currentDate = policy.StartDate;
            var monthlyPremium = policy.PremiumAmount / 12; // Assuming annual premium divided monthly

            int monthCount = 1;
            while (currentDate <= policy.EndDate)
            {
                payments.Add(new InsurancePayment
                {
                    InsurancePolicyId = policyId,
                    PaymentNumber = GeneratePaymentNumber(policyId, monthCount),
                    Amount = monthlyPremium,
                    DueDate = currentDate,
                    PaymentStatus = "Pending",
                    PaymentMethod = "Not Paid"
                });

                currentDate = currentDate.AddMonths(1);
                monthCount++;
            }

            await _context.InsurancePayments.AddRangeAsync(payments);
            await _context.SaveChangesAsync();
        }

        private string GeneratePaymentNumber(int policyId, int month)
        {
            return $"PMT-{policyId:D4}-{month:D2}-{DateTime.Now:yyyyMM}";
        }

        public async Task<List<InsurancePayment>> GetPaymentHistory(int policyId)
        {
            return await _context.InsurancePayments
                .Where(p => p.InsurancePolicyId == policyId)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<PaymentSummary> GetPaymentSummary(int policyId)
        {
            var payments = await _context.InsurancePayments
                .Where(p => p.InsurancePolicyId == policyId)
                .ToListAsync();

            var totalPremium = payments.Sum(p => p.Amount);
            var totalPaid = payments.Where(p => p.PaymentStatus == "Paid").Sum(p => p.Amount);
            var totalPending = payments.Where(p => p.PaymentStatus == "Pending").Sum(p => p.Amount);
            var totalOverdue = payments.Where(p => p.PaymentStatus == "Pending" && p.DueDate < DateTime.Now).Sum(p => p.Amount);
            var nextDue = payments.Where(p => p.PaymentStatus == "Pending")
                                  .OrderBy(p => p.DueDate)
                                  .FirstOrDefault();

            return new PaymentSummary
            {
                TotalPremium = totalPremium,
                TotalPaid = totalPaid,
                TotalPending = totalPending,
                TotalOverdue = totalOverdue,
                NextDueDate = nextDue?.DueDate,
                PaymentCount = payments.Count,
                PaidCount = payments.Count(p => p.PaymentStatus == "Paid")
            };
        }

        public async Task<InsurancePayment> RecordPremiumPayment(int policyId, decimal amount, string paymentMethod, string referenceNumber = "")
        {
            // Find the next pending payment
            var pendingPayment = await _context.InsurancePayments
                .Where(p => p.InsurancePolicyId == policyId && p.PaymentStatus == "Pending")
                .OrderBy(p => p.DueDate)
                .FirstOrDefaultAsync();

            if (pendingPayment == null)
                throw new Exception("No pending payments found for this policy");

            // Record the payment
            if (amount < pendingPayment.Amount)
            {
                // Partial payment
                pendingPayment.PaymentStatus = "Partial";
                pendingPayment.Remarks = $"Partial payment of {amount:C2} received";
                pendingPayment.PaymentDate = DateTime.Now;
                pendingPayment.PaymentMethod = paymentMethod;
                pendingPayment.ReferenceNumber = referenceNumber;
            }
            else if (amount >= pendingPayment.Amount)
            {
                // Full payment
                pendingPayment.PaymentStatus = "Paid";
                pendingPayment.PaymentDate = DateTime.Now;
                pendingPayment.PaymentMethod = paymentMethod;
                pendingPayment.ReferenceNumber = referenceNumber;

                // If overpaid, create credit for next payment
                if (amount > pendingPayment.Amount)
                {
                    var overpayment = amount - pendingPayment.Amount;
                    pendingPayment.Remarks = $"Overpayment of {overpayment:C2} applied to next payment";

                    // Apply overpayment to next pending payment
                    var nextPayment = await _context.InsurancePayments
                        .Where(p => p.InsurancePolicyId == policyId && p.PaymentStatus == "Pending"
                               && p.InsurancePaymentId != pendingPayment.InsurancePaymentId)
                        .OrderBy(p => p.DueDate)
                        .FirstOrDefaultAsync();

                    if (nextPayment != null)
                    {
                        nextPayment.Amount -= overpayment;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Update policy status
            await UpdatePolicyStatus(policyId);

            return pendingPayment;
        }

        #endregion

        #region Claim Management

        public async Task<InsuranceClaim> FileClaimAsync(InsuranceClaim claim)
        {
            // Get the policy with related data
            var policy = await _context.InsurancePolicies
                .Include(p => p.Payments)
                .Include(p => p.Claims)
                .FirstOrDefaultAsync(p => p.InsurancePolicyId == claim.InsurancePolicyId);

            if (policy == null)
                throw new Exception("Insurance policy not found");

            // Validate policy can file claims
            if (policy.Status != InsurancePolicyStatus.Active &&
                policy.Status != InsurancePolicyStatus.PaidUp)
            {
                throw new Exception($"Cannot file claim: Policy is {policy.Status}. Only Active or Fully Paid policies can file claims.");
            }

            // Check if payments are up to date
            var hasOverdue = policy.Payments.Any(p => p.PaymentStatus == "Pending" && p.DueDate < DateTime.Now);
            if (hasOverdue)
                throw new Exception("Cannot file claim: Policy has overdue payments");

            // Check if claim amount exceeds coverage
            if (claim.ClaimAmount > policy.CoverageAmount)
                throw new Exception($"Claim amount ({claim.ClaimAmount:C2}) exceeds coverage ({policy.CoverageAmount:C2})");

            // Check total claims already filed
            var existingClaimsTotal = policy.Claims
                .Where(c => c.Status == "Pending" || c.Status == "Approved")
                .Sum(c => c.ClaimAmount);

            if (existingClaimsTotal + claim.ClaimAmount > policy.CoverageAmount)
                throw new Exception($"Total claims ({existingClaimsTotal + claim.ClaimAmount:C2}) would exceed coverage ({policy.CoverageAmount:C2})");

            // Create the claim
            claim.ClaimNumber = GenerateClaimNumber();
            claim.FilingDate = DateTime.Now;
            claim.Status = "Pending";

            await _context.InsuranceClaims.AddAsync(claim);
            await _context.SaveChangesAsync();

            return claim;
        }

        public async Task<InsuranceClaim> ProcessClaimAsync(int claimId, bool approve, decimal? approvedAmount = null, string remarks = "")
        {
            var claim = await _context.InsuranceClaims
                .Include(c => c.InsurancePolicy)
                .FirstOrDefaultAsync(c => c.InsuranceClaimId == claimId);

            if (claim == null)
                throw new Exception("Claim not found");

            claim.Status = approve ? "Approved" : "Rejected";
            claim.ProcessedDate = DateTime.Now;
            claim.Remarks = remarks;

            if (approve)
            {
                // Use approved amount or full claim amount
                claim.ApprovedAmount = approvedAmount ?? claim.ClaimAmount;

                // Optional: Update policy if claim is for total loss
                if (claim.ApprovedAmount >= claim.InsurancePolicy.CoverageAmount)
                {
                    claim.InsurancePolicy.Status = InsurancePolicyStatus.Claimed;
                    claim.InsurancePolicy.StatusRemarks = $"Claim paid: {claim.ApprovedAmount:C2} on {DateTime.Now:d}";
                }
            }

            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<IEnumerable<InsuranceClaim>> GetAllClaimsAsync()
        {
            return await _context.InsuranceClaims
                .Include(c => c.InsurancePolicy)
                    .ThenInclude(p => p.Borrower)
                .OrderByDescending(c => c.FilingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InsuranceClaim>> GetClaimsByPolicyIdAsync(int policyId)
        {
            return await _context.InsuranceClaims
                .Where(c => c.InsurancePolicyId == policyId)
                .Include(c => c.InsurancePolicy)
                .OrderByDescending(c => c.FilingDate)
                .ToListAsync();
        }

        public async Task<InsuranceClaim> GetClaimByIdAsync(int claimId)
        {
            return await _context.InsuranceClaims
                .Include(c => c.InsurancePolicy)
                    .ThenInclude(p => p.Borrower)
                .FirstOrDefaultAsync(c => c.InsuranceClaimId == claimId);
        }

        private string GenerateClaimNumber()
        {
            var lastClaim = _context.InsuranceClaims
                .OrderByDescending(c => c.InsuranceClaimId)
                .FirstOrDefault();

            int nextNumber = (lastClaim?.InsuranceClaimId ?? 0) + 1;
            return $"CLM-{DateTime.Now:yyyyMMdd}-{nextNumber:D4}";
        }

        #endregion
    }

    // Payment summary DTO
    public class PaymentSummary
    {
        public decimal TotalPremium { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalOverdue { get; set; }
        public DateTime? NextDueDate { get; set; }
        public int PaymentCount { get; set; }
        public int PaidCount { get; set; }
    }
}