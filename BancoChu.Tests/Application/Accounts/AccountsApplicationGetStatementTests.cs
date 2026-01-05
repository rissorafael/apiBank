using BancoChu.Application;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using BancoChu.Domain.Interfaces;
using Moq;


namespace BancoChu.Tests.Application.Accounts
{

    public class AccountsApplicationGetStatementTests
    {
        private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
        private readonly Mock<IBankTransferRepository> _bankTransferRepositoryMock;
        private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IBusinessDayApplication> _businessDayApplicationMock;
        private readonly AccountsApplication _sut;

        public AccountsApplicationGetStatementTests()
        {
            _accountsRepositoryMock = new Mock<IAccountsRepository>();
            _bankTransferRepositoryMock = new Mock<IBankTransferRepository>();
            _connectionFactoryMock = new Mock<IDbConnectionFactory>();
            _businessDayApplicationMock = new Mock<IBusinessDayApplication>();

            _sut = new AccountsApplication(
                _accountsRepositoryMock.Object,
                _bankTransferRepositoryMock.Object,
                _connectionFactoryMock.Object,
                _businessDayApplicationMock.Object
            );
        }

        [Fact]
        public async Task GetStatementAsync_ShouldThrow_WhenAccountNotFound()
        {
            var accountId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _accountsRepositoryMock
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync((BankAccount?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.GetStatementAsync(userId, accountId, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow));
        }

        [Fact]
        public async Task GetStatementAsync_ShouldReturnStatements_WithNegativeAmountForOrigin()
        {
            var accountId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var account = BankAccount.Create(
                "12345",
                "0001",
                userId,
                1000,
                AccountType.Checking
            );

            _accountsRepositoryMock
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            var transferOrigin = BankTransfer.Create(accountId, Guid.NewGuid(), 200);
            var transferDestination = BankTransfer.Create(Guid.NewGuid(), accountId, 300);

            _bankTransferRepositoryMock
                .Setup(r => r.GetStatementAsync(accountId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<BankTransfer> { transferOrigin, transferDestination });

            var result = await _sut.GetStatementAsync(
                userId,
                accountId,
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow
            );

            var list = new List<BankTransfer>(result);

            Assert.Equal(-200, list[0].Amount);
            Assert.Equal(300, list[1].Amount);
        }

    }
}