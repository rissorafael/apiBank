using BancoChu.Application;
using BancoChu.Application.Dtos.Accounts;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using BancoChu.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Data;

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
                await _application.TransferAsync(Guid.NewGuid(), Guid.NewGuid(), request);

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
            var userId = Guid.NewGuid();

            var request = new TransferRequestDto
            {
                DestinationAccountId = accountId,
                Amount = 50
            };

            Func<Task> act = async () =>
                await _application.TransferAsync(userId, accountId, request);

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
                await _application.TransferAsync(Guid.NewGuid(), Guid.NewGuid(), request);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Conta de origem não encontrada.");
        }

    }
}