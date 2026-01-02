
using BancoChu.Domain.Entities;
using System.Data;

namespace BancoChu.Domain.Interfaces
{
    public interface IAccountsRepository
    {
        Task AddAsync(BankAccount account);
        Task<BankAccount?> GetByIdAsync(Guid accountId);
        Task UpdateBalanceAsync(Guid accountId, decimal newBalance, IDbTransaction transaction);
    }
}
