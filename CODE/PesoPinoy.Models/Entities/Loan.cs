using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.Models.Entities
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        [StringLength(20)]
        public string LoanNumber { get; set; }

        [Required]
        public int BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public virtual Borrower Borrower { get; set; }

        [Required]
        public decimal PrincipalAmount { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public int TermInMonths { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        public DateTime? ApprovalDate { get; set; }

        public DateTime? DisbursementDate { get; set; }

        [Required]
        public DateTime FirstPaymentDate { get; set; }

        public DateTime? MaturityDate { get; set; }

        [Required]
        public decimal MonthlyPayment { get; set; }

        public decimal TotalInterest { get; set; }

        public decimal TotalAmountPayable { get; set; }

        public decimal BalanceRemaining { get; set; }

        public LoanStatus Status { get; set; } = LoanStatus.Pending;

        public string Purpose { get; set; }

        public int? InsurancePolicyId { get; set; }

        [ForeignKey("InsurancePolicyId")]
        public virtual InsurancePolicy InsurancePolicy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<AmortizationSchedule> AmortizationSchedules { get; set; }
    }
}