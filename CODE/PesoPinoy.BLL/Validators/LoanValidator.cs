using PesoPinoy.Models.Entities;

namespace PesoPinoy.BLL.Validators
{
    public static class LoanValidator
    {
        public static (bool IsValid, string ErrorMessage) Validate(Loan loan)
        {
            if (loan.BorrowerId <= 0)
                return (false, "Invalid borrower selected");

            if (loan.PrincipalAmount < 1000)
                return (false, "Loan amount must be at least ₱1,000");

            if (loan.PrincipalAmount > 500000)
                return (false, "Loan amount cannot exceed ₱500,000");

            if (loan.TermInMonths < 1 || loan.TermInMonths > 60)
                return (false, "Loan term must be between 1 and 60 months");

            if (loan.InterestRate < 0 || loan.InterestRate > 100)
                return (false, "Invalid interest rate");

            if (loan.FirstPaymentDate < DateTime.Today)
                return (false, "First payment date cannot be in the past");

            if (string.IsNullOrWhiteSpace(loan.Purpose))
                return (false, "Loan purpose is required");

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidatePayment(Payment payment)
        {
            if (payment.LoanId <= 0)
                return (false, "Invalid loan selected");

            if (payment.AmountPaid <= 0)
                return (false, "Payment amount must be greater than 0");

            if (payment.PaymentDate < DateTime.Today.AddDays(-30))
                return (false, "Payment date is too far in the past");

            if (payment.PaymentDate > DateTime.Today.AddDays(30))
                return (false, "Payment date cannot be in the future");

            return (true, string.Empty);
        }
    }
}