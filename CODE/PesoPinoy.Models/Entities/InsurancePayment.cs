// PesoPinoy.Models/Entities/InsurancePayment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesoPinoy.Models.Entities
{
    public class InsurancePayment
    {
        [Key]
        public int InsurancePaymentId { get; set; }

        [Required]
        public int InsurancePolicyId { get; set; }

        [ForeignKey("InsurancePolicyId")]
        public virtual InsurancePolicy InsurancePolicy { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentNumber { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } // "Pending", "Paid", "Overdue", "Partial"

        [StringLength(20)]
        public string PaymentMethod { get; set; } // "Cash", "Bank Transfer", "Check", "Online"

        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }
    }
}