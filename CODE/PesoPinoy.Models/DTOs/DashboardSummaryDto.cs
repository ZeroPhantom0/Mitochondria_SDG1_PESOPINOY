namespace PesoPinoy.Models.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalBorrowers { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int CompletedLoans { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalCollections { get; set; }
        public decimal TotalSavings { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal AverageRiskScore { get; set; }
        public int HighRiskBorrowers { get; set; }
    }
}