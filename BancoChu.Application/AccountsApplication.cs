using BancoChu.Application.Dtos.Accounts;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;

namespace BancoChu.Application
{
    public class AccountsApplication : IAccountsApplication
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        public AccountsApplication(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
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

            await _bankAccountRepository.AddAsync(account);

            return account.Id;
        }
    }
}
