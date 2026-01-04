using BancoChu.Application;
using BancoChu.Application.Dtos.Accounts;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using BancoChu.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancoChu.Tests.Application.Accounts
{
    public class AccountsApplicationCreateTests
    {
        private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
        private readonly Mock<IBankTransferRepository> _bankTransferRepositoryMock;
        private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IBusinessDayApplication> _businessDayApplicationMock;

        private readonly AccountsApplication _accountsApplication;

        public AccountsApplicationCreateTests()
        {
            _accountsRepositoryMock = new Mock<IAccountsRepository>();
            _bankTransferRepositoryMock = new Mock<IBankTransferRepository>();
            _connectionFactoryMock = new Mock<IDbConnectionFactory>();
            _businessDayApplicationMock = new Mock<IBusinessDayApplication>();

            _accountsApplication = new AccountsApplication(
                _accountsRepositoryMock.Object,
                _bankTransferRepositoryMock.Object,
                _connectionFactoryMock.Object,
                _businessDayApplicationMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_Deve_Criar_Conta_Quando_Nao_Existir()
        {
            // Arrange
            var request = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = 1000m,
                Type = AccountType.Checking
            };

            _accountsRepositoryMock
                .Setup(r => r.ExistsByUserAndTypeAsync(request.UserId, request.Type))
                .ReturnsAsync(false);

            BankAccount? accountSaved = null;

            _accountsRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<BankAccount>()))
                .Callback<BankAccount>(acc => accountSaved = acc)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _accountsApplication.CreateAsync(request);

            // Assert
            result.Should().NotBe(Guid.Empty);

            accountSaved.Should().NotBeNull();
            accountSaved!.UserId.Should().Be(request.UserId);
            accountSaved.Type.Should().Be(request.Type);
            accountSaved.Balance.Should().Be(request.Balance);
            accountSaved.Status.Should().Be(AccountStatus.Active);

            _accountsRepositoryMock.Verify(
                r => r.ExistsByUserAndTypeAsync(request.UserId, request.Type),
                Times.Once
            );

            _accountsRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<BankAccount>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_Deve_Lancar_Excecao_Quando_Conta_Ja_Existir()
        {
            // Arrange
            var request = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = 500m,
                Type = AccountType.Savings
            };

            _accountsRepositoryMock
                .Setup(r => r.ExistsByUserAndTypeAsync(request.UserId, request.Type))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () =>
                await _accountsApplication.CreateAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"O usuário já possui uma conta do tipo {request.Type}.");

            _accountsRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<BankAccount>()),
                Times.Never
            );
        }
    }
}