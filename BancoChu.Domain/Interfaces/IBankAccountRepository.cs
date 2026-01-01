
using BancoChu.Domain.Entities;

namespace BancoChu.Domain.Interfaces
{
    public interface IBankAccountRepository
    {
        Task AddAsync(BankAccount account);
    }
}
