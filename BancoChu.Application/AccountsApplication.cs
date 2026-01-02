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
        public AccountsApplication(IAccountsRepository accountsRepository, IBankTransferRepository bankTransferRepository)
        {
            _accountsRepository = accountsRepository;
            _bankTransferRepository = bankTransferRepository;
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

        public async Task<Guid> TransferAsync(Guid accountId, TransferRequestDto request)
        {
            if (accountId == request.DestinationAccountId)
                throw new ArgumentException("Conta de origem e destino não podem ser iguais.");

            var originAccount = await _accountsRepository.GetByIdAsync(accountId) ??
                throw new InvalidOperationException("Conta de origem não encontrada.");

            var destinationAccount = await _accountsRepository.GetByIdAsync(request.DestinationAccountId) ??
                throw new InvalidOperationException("Conta de destino não encontrada.");

            if (originAccount.Status != AccountStatus.Active)
                throw new InvalidOperationException("Conta de origem não está ativa.");

            if (originAccount.Balance < request.Amount)
                throw new InvalidOperationException("Saldo insuficiente.");

            var newOriginBalance = originAccount.Balance - request.Amount;
            var newDestinationBalance = destinationAccount.Balance + request.Amount;

            await _accountsRepository.UpdateBalanceAsync(originAccount.Id, newOriginBalance);

            await _accountsRepository.UpdateBalanceAsync(destinationAccount.Id, newDestinationBalance);

            var transfer = BankTransfer.Create(
                                        accountId,
                                        request.DestinationAccountId,
                                        request.Amount);

            var transferId = await _bankTransferRepository.TransferAsync(transfer);
            return transferId;
        }
    }
}
