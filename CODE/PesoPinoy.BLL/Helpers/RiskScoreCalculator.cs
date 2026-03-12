using System;
using System.Linq;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.BLL.Helpers
{
    public static class RiskScoreCalculator
    {
        public static decimal CalculateRiskScore(Borrower borrower)
        {
            decimal score = 0;

            // Factor 1: Income level (0-30 points)
            if (borrower.MonthlyIncome >= 40000)
                score += 10;
            else if (borrower.MonthlyIncome >= 20000)
                score += 20;
            else if (borrower.MonthlyIncome >= 5000)
                score += 25;
            else
                score += 30;

            // Factor 2: Employment status (0-20 points)
            switch (borrower.EmploymentStatus)
            {
                case EmploymentStatus.Employed:
                    score += 5;
                    break;
                case EmploymentStatus.SelfEmployed:
                    score += 10;
                    break;
                case EmploymentStatus.Unemployed:
                    score += 20;
                    break;
                case EmploymentStatus.Student:
                    score += 15;
                    break;
                case EmploymentStatus.Retired:
                    score += 10;
                    break;
            }

            // Factor 3: Age (0-15 points)
            int age = DateTime.Now.Year - borrower.DateOfBirth.Year;
            if (age >= 25 && age <= 55)
                score += 5;
            else if (age < 25 || age > 55)
                score += 10;
            else
                score += 15;

            // Factor 4: Guarantor presence (0-15 points)
            if (!string.IsNullOrEmpty(borrower.GuarantorName))
                score += 5;
            else
                score += 15;

            // Factor 5: Payment history (0-20 points)
            // This would be calculated based on previous loans if any
            // For new borrowers, assign average score
            score += 10;

            return score;
        }

        public static RiskClassification ClassifyRisk(decimal riskScore)
        {
            if (riskScore <= 40)
                return RiskClassification.Low;
            else if (riskScore <= 60)
                return RiskClassification.Medium;
            else if (riskScore <= 80)
                return RiskClassification.High;
            else
                return RiskClassification.VeryHigh;
        }
    }
}