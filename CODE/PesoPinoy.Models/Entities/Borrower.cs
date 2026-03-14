using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.Models.Entities
{
    public class Borrower
    {
        [Key]
        public int BorrowerId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string MiddleName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public EmploymentStatus EmploymentStatus { get; set; }

        [Required]
        public decimal MonthlyIncome { get; set; }

        [StringLength(100)]
        public string EmployerName { get; set; }

        [StringLength(50)]
        public string GuarantorName { get; set; }

        [StringLength(20)]
        public string GuarantorContact { get; set; }

        [StringLength(200)]
        public string ReasonForLoan { get; set; }

        public decimal RiskScore { get; set; }

        public RiskClassification RiskClassification { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<SavingsAccount> SavingsAccounts { get; set; }
        public virtual ICollection<InsurancePolicy> InsurancePolicies { get; set; }
    }
}
