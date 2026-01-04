using BancoChu.Application;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;
using Moq;


namespace BancoChu.Tests.Application.Accounts
{
    public class AccountsApplicationGetStatementTests
    {
        private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
        private readonly Mock<IBankTransferRepository> _bankTransferRepositoryMock;
        private readonly AccountsApplication _application;

        public AccountsApplicationGetStatementTests()
        {
            _accountsRepositoryMock = new Mock<IAccountsRepository>();
            _bankTransferRepositoryMock = new Mock<IBankTransferRepository>();

            _application = new AccountsApplication(
                _accountsRepositoryMock.Object,
                _bankTransferRepositoryMock.Object,
                null!, // não usado nesse método
                null!  // não usado nesse método
            );
        }

        [Fact]
        public async Task GetStatementAsync_WhenAccountDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            _accountsRepositoryMock
                .Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync((BankAccount?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _application.GetStatementAsync(accountId, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow));

            Assert.Equal("Conta não encontrada.", ex.Message);
        }

        [Fact]
        public async Task GetStatementAsync_WhenOriginAccount_ShouldReturnNegativeAmount()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var transferAmount = 100m;

            var transfers = new List<BankTransfer>
            {
                BankTransfer.Create(
                    originAccountId: accountId,
                    destinationAccountId: Guid.NewGuid(),
                    amount: transferAmount
                )
            };

            _accountsRepositoryMock
                .Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync(Mock.Of<BankAccount>());

            _bankTransferRepositoryMock
                .Setup(x => x.GetStatementAsync(accountId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transfers);

            // Act
            var result = (await _application.GetStatementAsync(
                accountId,
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow
            )).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(-transferAmount, result[0].Amount);
        }

        [Fact]
        public async Task GetStatementAsync_WhenDestinationAccount_ShouldKeepPositiveAmount()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var transferAmount = 200m;

            var transfers = new List<BankTransfer>
            {
                BankTransfer.Create(
                    originAccountId: Guid.NewGuid(),
                    destinationAccountId: accountId,
                    amount: transferAmount
                )
            };

            _accountsRepositoryMock
                .Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync(Mock.Of<BankAccount>());

            _bankTransferRepositoryMock
                .Setup(x => x.GetStatementAsync(accountId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transfers);

            // Act
            var result = (await _application.GetStatementAsync(
                accountId,
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow
            )).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(transferAmount, result[0].Amount);
        }

        [Fact]
        public async Task GetStatementAsync_WhenNoTransfers_ShouldReturnEmptyList()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            _accountsRepositoryMock
                .Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync(Mock.Of<BankAccount>());

            _bankTransferRepositoryMock
                .Setup(x => x.GetStatementAsync(accountId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<BankTransfer>());

            // Act
            var result = await _application.GetStatementAsync(
                accountId,
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow
            );

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}