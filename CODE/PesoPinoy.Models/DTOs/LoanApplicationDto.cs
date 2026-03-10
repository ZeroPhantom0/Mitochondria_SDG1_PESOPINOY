using System;

namespace PesoPinoy.Models.DTOs
{
    public class LoanApplicationDto
    {
        public int BorrowerId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public int TermInMonths { get; set; }
        public DateTime FirstPaymentDate { get; set; }
        public string Purpose { get; set; }
        public int? InsurancePolicyId { get; set; }
    }
}