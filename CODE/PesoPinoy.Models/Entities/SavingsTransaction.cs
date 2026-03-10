using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesoPinoy.Models.Entities
{
    public class SavingsTransaction
    {
        [Key]
        public int SavingsTransactionId { get; set; }

        [Required]
        public int SavingsAccountId { get; set; }

        [ForeignKey("SavingsAccountId")]
        public virtual SavingsAccount SavingsAccount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } // Deposit, Withdrawal, Interest

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal BalanceBefore { get; set; }

        [Required]
        public decimal BalanceAfter { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        public string ProcessedBy { get; set; }
    }
}