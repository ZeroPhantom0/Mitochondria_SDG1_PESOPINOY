using System;
using System.ComponentModel.DataAnnotations;

namespace PesoPinoy.Models.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } // Create, Update, Delete, Login, etc.

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } // Borrower, Loan, Payment, etc.

        public int? EntityId { get; set; }

        [StringLength(4000)]
        public string OldValues { get; set; }

        [StringLength(4000)]
        public string NewValues { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }
    }
}