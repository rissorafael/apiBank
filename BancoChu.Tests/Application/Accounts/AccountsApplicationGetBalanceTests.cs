using BancoChu.Application;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using BancoChu.Domain.Interfaces;
using FluentAssertions;
using Moq;


namespace BancoChu.Tests.Application.Accounts
{
    public class AccountsApplicationGetBalanceTests
    {
        private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
        private readonly AccountsApplication _sut;

        public AccountsApplicationGetBalanceTests()
        {
            _accountsRepositoryMock = new Mock<IAccountsRepository>();

            _sut = new AccountsApplication(
                _accountsRepositoryMock.Object,
                bankTransferRepository: null!,
                connectionFactory: null!,
                businessDayApplication: null!
            );
        }

        [Fact]
        public async Task GetBalanceAsync_ShouldReturnBalance_WhenAccountBelongsToUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var account = BankAccount.Create(
                accountNumber: "123456",
                agency: "0001",
                userId: userId,
                balance: 1500m,
                type: AccountType.Checking
            );

            _accountsRepositoryMock
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            // Act
            var balance = await _sut.GetBalanceAsync(userId, accountId);

            // Assert
            balance.Should().Be(1500m);
        }
        [Fact]
        public async Task GetBalanceAsync_ShouldThrowInvalidOperationException_WhenAccountDoesNotBelongToUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var account = BankAccount.Create(
                accountNumber: "123456",
                agency: "0001",
                userId: otherUserId,
                balance: 1000m,
                type: AccountType.Checking
            );

            _accountsRepositoryMock
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            // Act
            Func<Task> act = () => _sut.GetBalanceAsync(userId, accountId);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("A conta de origem não pertence ao usuário logado.");
        }
    }
}
