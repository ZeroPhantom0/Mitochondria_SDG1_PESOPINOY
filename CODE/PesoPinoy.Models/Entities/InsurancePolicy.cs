using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.Models.Entities
{
    public class InsurancePolicy
    {
        [Key]
        public int InsurancePolicyId { get; set; }

        [Required]
        [StringLength(20)]
        public string PolicyNumber { get; set; }

        [Required]
        public int BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public virtual Borrower Borrower { get; set; }

        [Required]
        public string PolicyType { get; set; } // Health, Life, Education

        [Required]
        public decimal CoverageAmount { get; set; }

        [Required]
        public decimal PremiumAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryRelation { get; set; }

        // New Status Enum instead of bool IsActive
        public InsurancePolicyStatus Status { get; set; } = InsurancePolicyStatus.Pending;

        // For backward compatibility
        [NotMapped]
        public bool IsActive
        {
            get { return Status == InsurancePolicyStatus.Active || Status == InsurancePolicyStatus.PaidUp; }
        }

        // New field for remarks
        [StringLength(500)]
        public string StatusRemarks { get; set; }

        // Navigation Properties
        public virtual ICollection<InsuranceClaim> Claims { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<InsurancePayment> Payments { get; set; } // New navigation property
    }
}