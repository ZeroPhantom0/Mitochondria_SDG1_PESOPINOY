using System;
using System.Text.RegularExpressions;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.BBL.Validators
{
    public static class BorrowerValidator
    {
        public static (bool IsValid, string ErrorMessage) Validate(Borrower borrower)
        {
            if (string.IsNullOrWhiteSpace(borrower.FirstName))
                return (false, "First name is required");

            if (string.IsNullOrWhiteSpace(borrower.LastName))
                return (false, "Last name is required");

            if (borrower.DateOfBirth > DateTime.Today.AddYears(-18))
                return (false, "Borrower must be at least 18 years old");

            if (string.IsNullOrWhiteSpace(borrower.ContactNumber) ||
                !Regex.IsMatch(borrower.ContactNumber, @"^[0-9]{11}$"))
                return (false, "Please enter a valid 11-digit contact number");

            if (string.IsNullOrWhiteSpace(borrower.Email) ||
                !Regex.IsMatch(borrower.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Please enter a valid email address");

            if (borrower.MonthlyIncome <= 0)
                return (false, "Monthly income must be greater than 0");

            return (true, string.Empty);
        }
    }
}
