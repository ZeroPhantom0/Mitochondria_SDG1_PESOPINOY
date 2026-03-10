using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesoPinoy.Models.Entities
{
    public class SavingsAccount
    {
        [Key]
        public int SavingsAccountId { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        public int BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public virtual Borrower Borrower { get; set; }

        [Required]
        public decimal CurrentBalance { get; set; }

        public decimal InterestRate { get; set; } = 0.25m; // 0.25% monthly

        public DateTime OpenedDate { get; set; } = DateTime.Now;

        public DateTime LastTransactionDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<SavingsTransaction> Transactions { get; set; }
    }
}