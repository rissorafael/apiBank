using BancoChu.Domain.Entities;
using System.Data;

namespace BancoChu.Domain.Interfaces
{
    public interface IBankTransferRepository
    {
        Task<Guid> TransferAsync(BankTransfer transfer, IDbTransaction transaction);
        Task<IEnumerable<BankTransfer>> GetStatementAsync(Guid accountId, DateTime startDate, DateTime endDate);
    }
}
