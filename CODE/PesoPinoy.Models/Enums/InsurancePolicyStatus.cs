namespace PesoPinoy.Models.Enums
{
    public enum InsurancePolicyStatus
    {
        Pending,      // Applied, waiting for approval/first payment
        Active,       // Policy is active and providing coverage
        PaidUp,       // Fully paid but still within coverage period
        Lapsed,       // Missed payments, coverage stopped
        Expired,      // Policy term ended
        Cancelled,    // Policy terminated early
        Claimed       // Benefit has been paid out
    }
}