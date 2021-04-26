using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;

namespace Moneybox.App.Test
{
    [TestClass]
    public class WithdrawMoenyTests
    {
        Mock<IAccountRepository> mockAccountRepository;

        Mock<INotificationService> mockNotificationService;

        Account account = new()
        {
            Balance = 1000,
            Id = new Guid(),
            User = new()
            {
                Email = "test@abc.com",
                Id = new Guid(),
                Name = "First Last"
            },
        };

        [TestInitialize]
        public void SetUp()
        {
            mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(account);

            mockNotificationService = new Mock<INotificationService>();
            mockNotificationService.Setup(x => x.NotifyFundsLow(It.IsAny<string>()));
        }

        [TestMethod]
        public void WithdrawSuccess()
        {
            WithdrawMoney withdraw = new(mockAccountRepository.Object, mockNotificationService.Object);

            withdraw.Execute(new Guid(), 100);
        }

        [TestMethod]
        public void WithdrawSuccess_ZeroBalance()
        {
            WithdrawMoney withdraw = new(mockAccountRepository.Object, mockNotificationService.Object);

            withdraw.Execute(new Guid(), 1000);
        }
         
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithdrawFailure()
        {
            WithdrawMoney withdraw = new(mockAccountRepository.Object, mockNotificationService.Object);

            withdraw.Execute(new Guid(), 1001);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Withdraw_GetAccountById_Fails()
        {
            mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Throws(new Exception());

            WithdrawMoney withdraw = new(mockAccountRepository.Object, mockNotificationService.Object);

            withdraw.Execute(new Guid(), 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Withdraw_Update_Fails()
        {
            mockAccountRepository.Setup(x => x.Update(It.IsAny<Account>())).Throws(new Exception());

            WithdrawMoney withdraw = new(mockAccountRepository.Object, mockNotificationService.Object);

            withdraw.Execute(new Guid(), 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Withdraw_NotifyFundsLow_Fails()
        {
            mockNotificationService.Setup(x => x.NotifyFundsLow(It.IsAny<string>())).Throws(new Exception());

            WithdrawMoney withdraw = new(mockAccountRepository.Object, mockNotificationService.Object);

            withdraw.Execute(new Guid(), 1000);
        }
    }
}
