using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository accountRepository;
        private readonly INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = accountRepository.GetAccountById(fromAccountId);
            var to = accountRepository.GetAccountById(toAccountId);

            from.CalculateWithdraw(amount);

            if (from.Balance < 500m)
            {
                notificationService.NotifyFundsLow(from.User.Email);
            }

            to.CalculatePaidIn(amount);

            if (Account.PayInLimit - to.PaidIn < 500m)
            {
                notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }

            accountRepository.Update(from);
            accountRepository.Update(to);
        }
    }
}
