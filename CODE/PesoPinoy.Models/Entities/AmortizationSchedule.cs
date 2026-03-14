using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PesoPinoy.Models.Enums;;

namespace PesoPinoy.Models.Entities
{
    public class AmortizationSchedule
    {
        [Key]
        public int AmortizationScheduleId { get; set; }

        [Required]
        public int LoanId { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        [Required]
        public int PaymentNumber { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [Required]
        public decimal BeginningBalance { get; set; }

        [Required]
        public decimal PaymentAmount { get; set; }

        [Required]
        public decimal PrincipalAmount { get; set; }

        [Required]
        public decimal InterestAmount { get; set; }

        [Required]
        public decimal EndingBalance { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string Remarks { get; set; }
    }
}
