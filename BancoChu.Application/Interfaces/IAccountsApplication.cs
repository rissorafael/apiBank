
using BancoChu.Application.Dtos.Accounts;
using BancoChu.Domain.Entities;

namespace BancoChu.Application.Interfaces
{
    public interface IAccountsApplication
    {
        Task<Guid> CreateAsync(CreateAccountsRequestDto dto);
        Task<Guid> TransferAsync(Guid accountId, TransferRequestDto request);
        Task<IEnumerable<BankTransfer>> GetStatementAsync(Guid accountId, DateTime startDate, DateTime endDate);
            //Task TransferAsync(TransferCommand command);
        //Task<AccountStatementDto> GetStatementAsync(GetStatementQuery query);
    }
}
