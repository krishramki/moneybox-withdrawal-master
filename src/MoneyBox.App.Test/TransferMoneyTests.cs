using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;

namespace Moneybox.App.Test
{
    [TestClass]
    public class TransferMoneyTests
    {
        Mock<IAccountRepository> mockAccountRepository;

        Mock<INotificationService> mockNotificationService;

        Guid from = new("1102F6F2-7960-4832-A533-F196EA71FB65");

        Guid to = new("877C2D2D-D96C-4048-8039-0CACE04CDF9A");

        Account fromAccount = new()
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

        Account toAccount = new()
        {
            Balance = 1000,
            Id = new Guid(),
            User = new()
            {
                Email = "SecondAccount@abc.com",
                Id = new Guid(),
                Name = "Second Account"
            },
        };

        [TestInitialize]
        public void SetUp()
        {
            mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(x=>x == from))).Returns(fromAccount);
            mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(x => x == to))).Returns(toAccount);

            mockNotificationService = new Mock<INotificationService>();
            mockNotificationService.Setup(x => x.NotifyFundsLow(It.IsAny<string>()));
            mockNotificationService.Setup(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()));
        }

        [TestMethod]
        public void TransferSuccess()
        {
            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 100);
        }

        [TestMethod]
        public void Transfer_NotifyApproachingPayInLimit()
        {
            fromAccount.Balance = 4000;
            mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(x => x == from))).Returns(fromAccount);

            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 3600);
        }

        [TestMethod]
        public void TransferSuccess_ZeroBalance()
        {
            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TransferFailure()
        {
            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 1001);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TransferPaidInFailure()
        {
            fromAccount.Balance = 4100;
            mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(x => x == from))).Returns(fromAccount);

            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 4001);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Transfer_GetAccountById_Fails()
        {
            mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Throws(new Exception());

            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Transfer_Update_Fails()
        {
            mockAccountRepository.Setup(x => x.Update(It.IsAny<Account>())).Throws(new Exception());

            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Transfer_NotifyFundsLow_Fails()
        {
            mockNotificationService.Setup(x => x.NotifyFundsLow(It.IsAny<string>())).Throws(new Exception());

            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Transfer_NotifyApproachingPayInLimit_Fails()
        {
            fromAccount.Balance = 4000;
            mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(x => x == from))).Returns(fromAccount);

            mockNotificationService.Setup(x => x.NotifyApproachingPayInLimit(It.IsAny<string>())).Throws(new Exception());

            TransferMoney transfer = new(mockAccountRepository.Object, mockNotificationService.Object);

            transfer.Execute(from, to, 3600);
        }
    }
}
