using System;

namespace PesoPinoy.BLL.Helpers
{
    public static class PenaltyCalculator
    {
        private const decimal DAILY_PENALTY_RATE = 0.005m; // 0.5% per day
        private const int GRACE_PERIOD_DAYS = 3;
        private const decimal MAX_PENALTY_RATE = 0.20m; // 20% maximum

        public static decimal CalculatePenalty(decimal amountDue, int daysLate)
        {
            if (daysLate <= GRACE_PERIOD_DAYS)
                return 0;

            int penaltyDays = daysLate - GRACE_PERIOD_DAYS;
            decimal penalty = amountDue * DAILY_PENALTY_RATE * penaltyDays;

            // Apply maximum penalty cap
            decimal maxPenalty = amountDue * MAX_PENALTY_RATE;
            return Math.Min(penalty, maxPenalty);
        }

        public static int CalculateDaysLate(DateTime dueDate, DateTime paymentDate)
        {
            if (paymentDate <= dueDate)
                return 0;

            return (paymentDate - dueDate).Days;
        }

        public static string GetPenaltyStatus(int daysLate)
        {
            if (daysLate <= 0)
                return "On Time";
            else if (daysLate <= GRACE_PERIOD_DAYS)
                return "Grace Period";
            else if (daysLate <= 15)
                return "Late (Minor)";
            else if (daysLate <= 30)
                return "Late (Major)";
            else
                return "Default";
        }
    }
}