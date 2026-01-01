
using BancoChu.Application.Dtos.Accounts;

namespace BancoChu.Application.Interfaces
{
    public interface IAccountsApplication
    {
        Task<Guid> CreateAsync(CreateAccountsRequestDto dto);
        //Task TransferAsync(TransferCommand command);
        //Task<AccountStatementDto> GetStatementAsync(GetStatementQuery query);
    }
}
