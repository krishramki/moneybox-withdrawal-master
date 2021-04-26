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

        public void Withdraw(decimal amount)
        {
            var withdrawalBalance = Balance - amount;

            if (withdrawalBalance < 0)
            {
                throw new InvalidOperationException("Insufficient funds in the account");
            }

            Balance -= amount;
            Withdrawn -= amount;
        }

        //public void Withdraw(decimal amount, string reference)
        //{
        //    if (CanWithdraw(amount))
        //    {
        //        Balance -= amount;
        //        Transaction = new Transaction(0m, amount,
        //        reference, DateTime.Now);
        //    }
        //    else
        //    {
        //        throw new Exception("");
        //    }
        //}
        //public void Deposit(decimal amount, string reference)
        //{
        //    Balance += amount;
        //    Transaction = new Transaction(amount, 0m, reference, DateTime.Now);
        //}
    }
}
