
using BancoChu.Application.Dtos.Accounts;
using BancoChu.Domain.Entities;

namespace BancoChu.Application.Interfaces
{
    public interface IAccountsApplication
    {
        Task<Guid> CreateAsync(CreateAccountsRequestDto dto);
        Task<Guid> TransferAsync(Guid userId, Guid accountId, TransferRequestDto request);
        Task<IEnumerable<BankTransfer>> GetStatementAsync(Guid userId, Guid accountId, DateTime startDate, DateTime endDate);
        Task<decimal> GetBalanceAsync(Guid userId, Guid accountId);
    }
}
