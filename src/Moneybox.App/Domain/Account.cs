using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void CalculateWithdraw(decimal amount)
        {
            Balance -= amount;
            Withdrawn -= amount;

            if (Balance < 0)
            {
                throw new InvalidOperationException("Insufficient funds in the account");
            }
        }

        public void CalculatePaidIn(decimal amount)
        {
            PaidIn += amount;
            Balance += amount;

            if (PaidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
        }
    }
}
