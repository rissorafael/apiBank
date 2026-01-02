
using BancoChu.Application.Dtos.Accounts;

namespace BancoChu.Application.Interfaces
{
    public interface IAccountsApplication
    {
        Task<Guid> CreateAsync(CreateAccountsRequestDto dto);
        Task<Guid> TransferAsync(Guid accountId, TransferRequestDto request);
        //Task TransferAsync(TransferCommand command);
        //Task<AccountStatementDto> GetStatementAsync(GetStatementQuery query);
    }
}
