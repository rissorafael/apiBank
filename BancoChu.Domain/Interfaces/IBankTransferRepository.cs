using BancoChu.Domain.Entities;

namespace BancoChu.Domain.Interfaces
{
    public interface IBankTransferRepository
    {
        Task<Guid> TransferAsync(BankTransfer transfer);
    }
}
