using System;

namespace PesoPinoy.Models.DTOs
{
    public class PaymentDto
    {
        public int LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Remarks { get; set; }
    }
}