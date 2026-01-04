
using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using System.Data;

namespace BancoChu.Domain.Interfaces
{
    public interface IAccountsRepository
    {
        Task<bool> ExistsByUserAndTypeAsync(Guid userId, AccountType type);
        Task AddAsync(BankAccount account);
        Task<BankAccount?> GetByIdAsync(Guid accountId);
        Task UpdateBalanceAsync(Guid accountId, decimal newBalance, IDbTransaction transaction);
    }
}
