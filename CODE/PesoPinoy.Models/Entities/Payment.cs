using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.Models.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentNumber { get; set; }

        [Required]
        public int LoanId { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal AmountDue { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }

        public decimal LatePenalty { get; set; }

        public int DaysLate { get; set; }

        public decimal PrincipalPaid { get; set; }

        public decimal InterestPaid { get; set; }

        public PaymentStatus Status { get; set; }

        public string PaymentMethod { get; set; }

        public string ReferenceNumber { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
    }
}