using BancoChu.Application.Dtos.Accounts;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using BancoChu.Domain.Interfaces;


namespace BancoChu.Application
{
    public class AccountsApplication : IAccountsApplication
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IBankTransferRepository _bankTransferRepository;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IBusinessDayApplication _businessDayApplication;
        public AccountsApplication(IAccountsRepository accountsRepository, IBankTransferRepository bankTransferRepository, IDbConnectionFactory connectionFactory, IBusinessDayApplication businessDayApplication)
        {
            _accountsRepository = accountsRepository;
            _bankTransferRepository = bankTransferRepository;
            _connectionFactory = connectionFactory;
            _businessDayApplication = businessDayApplication;
        }
        public async Task<Guid> CreateAsync(CreateAccountsRequestDto dto)
        {
            var account = BankAccount.Create(
             dto.AccountNumber,
             dto.Agency,
             dto.UserId,
             dto.Balance,
             dto.Type
         );

            await _accountsRepository.AddAsync(account);

            return account.Id;
        }

        public async Task<Guid> TransferAsync(Guid accountId, TransferRequestDto dto)
        {

            var today = DateTime.UtcNow.Date;

            var isBusinessDay = await _businessDayApplication.IsBusinessDayAsync(today);

            if (!isBusinessDay)
                throw new InvalidOperationException("Transferências só podem ser realizadas em dias úteis.");

            if (accountId == dto.DestinationAccountId)
                throw new ArgumentException("Conta de origem e destino não podem ser iguais.");

            var originAccount = await _accountsRepository.GetByIdAsync(accountId) ??
                throw new InvalidOperationException("Conta de origem não encontrada.");

            var destinationAccount = await _accountsRepository.GetByIdAsync(dto.DestinationAccountId) ??
                throw new InvalidOperationException("Conta de destino não encontrada.");

            if (originAccount.Status != AccountStatus.Active)
                throw new InvalidOperationException("Conta de origem não está ativa.");

            if (originAccount.Balance < dto.Amount)
                throw new InvalidOperationException("Saldo insuficiente.");

            var newOriginBalance = originAccount.Balance - dto.Amount;
            var newDestinationBalance = destinationAccount.Balance + dto.Amount;

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var transfer = BankTransfer.Create(originAccount.Id, dto.DestinationAccountId, dto.Amount);

                await _accountsRepository.UpdateBalanceAsync(originAccount.Id, newOriginBalance, transaction);

                await _accountsRepository.UpdateBalanceAsync(destinationAccount.Id, newDestinationBalance, transaction);

                var transferId = await _bankTransferRepository.TransferAsync(transfer, transaction);

                transaction.Commit();

                return transferId;
            }
            catch (Exception)
            {

                transaction.Rollback();
                throw;
            }
            finally
            {
                if (transaction.Connection != null)
                {
                    transaction.Connection.Dispose();
                }
            }
        }
    }
}
