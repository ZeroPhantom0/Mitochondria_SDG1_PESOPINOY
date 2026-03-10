using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesoPinoy.Models.Entities
{
    public class InsuranceClaim
    {
        [Key]
        public int InsuranceClaimId { get; set; }

        [Required]
        public int InsurancePolicyId { get; set; }

        [ForeignKey("InsurancePolicyId")]
        public virtual InsurancePolicy InsurancePolicy { get; set; }

        [Required]
        [StringLength(20)]
        public string ClaimNumber { get; set; }

        [Required]
        public DateTime FilingDate { get; set; }

        public DateTime? IncidentDate { get; set; }

        [Required]
        public decimal ClaimAmount { get; set; }

        [Required]
        [StringLength(500)]
        public string ClaimReason { get; set; }

        [StringLength(50)]
        public string Status { get; set; } // Pending, Approved, Rejected, Paid

        public DateTime? ProcessedDate { get; set; }

        public decimal? ApprovedAmount { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [StringLength(200)]
        public string SupportingDocuments { get; set; }
    }
}