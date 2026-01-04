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
using System.Data;
using System.Text;

namespace BancoChu.Tests.Application.Accounts
{
    public class AccountsApplicationTransferTests
    {
        private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
        private readonly Mock<IBankTransferRepository> _bankTransferRepositoryMock;
        private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IBusinessDayApplication> _businessDayApplicationMock;

        private readonly AccountsApplication _application;

        public AccountsApplicationTransferTests()
        {
            _accountsRepositoryMock = new Mock<IAccountsRepository>();
            _bankTransferRepositoryMock = new Mock<IBankTransferRepository>();
            _connectionFactoryMock = new Mock<IDbConnectionFactory>();
            _businessDayApplicationMock = new Mock<IBusinessDayApplication>();

            _application = new AccountsApplication(
                _accountsRepositoryMock.Object,
                _bankTransferRepositoryMock.Object,
                _connectionFactoryMock.Object,
                _businessDayApplicationMock.Object
            );
        }


        [Fact]
        public async Task TransferAsync_Deve_Falhar_Quando_Nao_For_Dia_Util()
        {
            _businessDayApplicationMock
                .Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            var request = new TransferRequestDto
            {
                DestinationAccountId = Guid.NewGuid(),
                Amount = 100
            };

            Func<Task> act = async () =>
                await _application.TransferAsync(Guid.NewGuid(), request);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Transferências só podem ser realizadas em dias úteis.");
        }

        [Fact]
        public async Task TransferAsync_Deve_Falhar_Quando_Conta_Origem_E_Destino_Forem_Iguais()
        {
            _businessDayApplicationMock
                .Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var accountId = Guid.NewGuid();

            var request = new TransferRequestDto
            {
                DestinationAccountId = accountId,
                Amount = 50
            };

            Func<Task> act = async () =>
                await _application.TransferAsync(accountId, request);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Conta de origem e destino não podem ser iguais.");
        }

        [Fact]
        public async Task TransferAsync_Deve_Falhar_Quando_Conta_Origem_Nao_Existir()
        {
            _businessDayApplicationMock
                .Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            _accountsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BankAccount?)null);

            var request = new TransferRequestDto
            {
                DestinationAccountId = Guid.NewGuid(),
                Amount = 100
            };

            Func<Task> act = async () =>
                await _application.TransferAsync(Guid.NewGuid(), request);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Conta de origem não encontrada.");
        }

        [Fact]
        public async Task TransferAsync_Deve_Falhar_Quando_Saldo_For_Insuficiente()
        {
            _businessDayApplicationMock
                .Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var origin = CreateAccount(Guid.NewGuid(), 50, AccountStatus.Active);
            var destination = CreateAccount(Guid.NewGuid(), 0, AccountStatus.Active);

            _accountsRepositoryMock
                .SetupSequence(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(origin)
                .ReturnsAsync(destination);

            var request = new TransferRequestDto
            {
                DestinationAccountId = destination.Id,
                Amount = 100
            };

            Func<Task> act = async () =>
                await _application.TransferAsync(origin.Id, request);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Saldo insuficiente.");
        }

        [Fact]
        public async Task TransferAsync_Deve_Executar_Transferencia_Com_Sucesso()
        {
            _businessDayApplicationMock
                .Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var origin = CreateAccount(Guid.NewGuid(), 500, AccountStatus.Active);
            var destination = CreateAccount(Guid.NewGuid(), 100, AccountStatus.Active);

            _accountsRepositoryMock
                .SetupSequence(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(origin)
                .ReturnsAsync(destination);

            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();

            transactionMock.Setup(t => t.Connection).Returns(connectionMock.Object);
            connectionMock.Setup(c => c.BeginTransaction()).Returns(transactionMock.Object);

            _connectionFactoryMock
                .Setup(x => x.CreateConnection())
                .Returns(connectionMock.Object);

            _bankTransferRepositoryMock
                .Setup(x => x.TransferAsync(It.IsAny<BankTransfer>(), transactionMock.Object))
                .ReturnsAsync(Guid.NewGuid());

            var request = new TransferRequestDto
            {
                DestinationAccountId = destination.Id,
                Amount = 100
            };

            var result = await _application.TransferAsync(origin.Id, request);

            result.Should().NotBe(Guid.Empty);

            _accountsRepositoryMock.Verify(
                x => x.UpdateBalanceAsync(origin.Id, 400, transactionMock.Object),
                Times.Once
            );

            _accountsRepositoryMock.Verify(
                x => x.UpdateBalanceAsync(destination.Id, 200, transactionMock.Object),
                Times.Once
            );

            _bankTransferRepositoryMock.Verify(
                x => x.TransferAsync(It.IsAny<BankTransfer>(), transactionMock.Object),
                Times.Once
            );
        }


        private static BankAccount CreateAccount(Guid id, decimal balance, AccountStatus status)
        {
            return typeof(BankAccount)
                .GetMethod("Create")!
                .Invoke(null, new object[]
                {
                    "12345", "0001", Guid.NewGuid(), balance, AccountType.Checking
                }) as BankAccount;
        }

    }
}