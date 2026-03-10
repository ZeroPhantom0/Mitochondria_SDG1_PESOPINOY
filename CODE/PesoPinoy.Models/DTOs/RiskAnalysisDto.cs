using System.Collections.Generic;

namespace PesoPinoy.Models.DTOs
{
    public class RiskAnalysisDto
    {
        public int TotalBorrowers { get; set; }
        public Dictionary<string, int> RiskDistribution { get; set; }
        public decimal HighRiskExposure { get; set; }
        public decimal RiskPercentage { get; set; }
        public List<HighRiskBorrowerDto> HighRiskBorrowers { get; set; }
    }

    public class HighRiskBorrowerDto
    {
        public int BorrowerId { get; set; }
        public string FullName { get; set; }
        public decimal RiskScore { get; set; }
        public string RiskClassification { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public int OverduePayments { get; set; }
    }
}