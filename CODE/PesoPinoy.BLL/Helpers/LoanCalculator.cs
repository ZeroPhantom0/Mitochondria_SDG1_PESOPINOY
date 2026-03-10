using System;
using System.Collections.Generic;

namespace PesoPinoy.BLL.Helpers
{
    public class LoanCalculationResult
    {
        public decimal MonthlyPayment { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class AmortizationRow
    {
        public int PaymentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal BeginningBalance { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal EndingBalance { get; set; }
    }

    public class LoanCalculator
    {
        public LoanCalculationResult CalculateLoan(decimal principal, decimal annualInterestRate, int termInMonths)
        {
            decimal monthlyRate = annualInterestRate / 100 / 12;

            // Monthly payment using PMT formula
            decimal monthlyPayment;

            if (monthlyRate == 0)
            {
                monthlyPayment = principal / termInMonths;
            }
            else
            {
                decimal factor = (decimal)Math.Pow(1 + (double)monthlyRate, termInMonths);
                monthlyPayment = principal * monthlyRate * factor / (factor - 1);
            }

            decimal totalAmount = monthlyPayment * termInMonths;
            decimal totalInterest = totalAmount - principal;

            return new LoanCalculationResult
            {
                MonthlyPayment = Math.Round(monthlyPayment, 2),
                TotalInterest = Math.Round(totalInterest, 2),
                TotalAmount = Math.Round(totalAmount, 2)
            };
        }

        public List<AmortizationRow> GenerateAmortizationSchedule(decimal principal, decimal annualInterestRate,
            int termInMonths, DateTime firstPaymentDate)
        {
            var schedule = new List<AmortizationRow>();
            decimal monthlyRate = annualInterestRate / 100 / 12;

            var result = CalculateLoan(principal, annualInterestRate, termInMonths);
            decimal monthlyPayment = result.MonthlyPayment;

            decimal balance = principal;
            DateTime dueDate = firstPaymentDate;

            for (int i = 1; i <= termInMonths; i++)
            {
                decimal interestAmount = balance * monthlyRate;
                decimal principalAmount = monthlyPayment - interestAmount;

                if (i == termInMonths)
                {
                    principalAmount = balance;
                    monthlyPayment = principalAmount + interestAmount;
                }

                decimal beginningBalance = balance;
                balance -= principalAmount;

                schedule.Add(new AmortizationRow
                {
                    PaymentNumber = i,
                    DueDate = dueDate,
                    BeginningBalance = Math.Round(beginningBalance, 2),
                    PaymentAmount = Math.Round(monthlyPayment, 2),
                    PrincipalAmount = Math.Round(principalAmount, 2),
                    InterestAmount = Math.Round(interestAmount, 2),
                    EndingBalance = Math.Round(balance, 2)
                });

                dueDate = dueDate.AddMonths(1);
            }

            return schedule;
        }
    }
}