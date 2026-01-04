using BancoChu.Domain.Entities;
using BancoChu.Domain.Enums;
using BancoChu.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BancoChu.Infrastructure.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AccountsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> ExistsByUserAndTypeAsync(Guid userId, AccountType type)
        {
            const string sql = @"
                             SELECT 1
                              FROM bank_accounts
                               WHERE user_id = @userId
                             AND type = @type
                             LIMIT 1;";

            using var connection = _connectionFactory.CreateConnection();

            var exists = await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { userId, type }
            );

            return exists.HasValue;
        }

        public async Task AddAsync(BankAccount account)
        {
            const string sql = @"
            INSERT INTO bank_accounts
            (
                id,
                account_number,
                agency,
                user_id,
                balance,
                status,
                type,
                created_at
            )
            VALUES
            (
                @id,
                @AccountNumber,
                @Agency,
                @UserId,
                @Balance,
                @Status,
                @Type,
                @CreatedAt
            );";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                id = account.Id,
                AccountNumber = account.AccountNumber,
                Agency = account.Agency,
                UserId = account.UserId,
                Balance = account.Balance,
                Status = (int)account.Status,
                Type = (int)account.Type,
                CreatedAt = account.CreatedAt
            });
        }

        public async Task<BankAccount?> GetByIdAsync(Guid accountId)
        {
            const string sql = @"
                          SELECT
                              id,
                              balance,
                              status
                          FROM bank_accounts
                          WHERE id = @accountId;";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<BankAccount>(sql,
                new { accountId });

            return result;
        }


        public async Task UpdateBalanceAsync(Guid accountId, decimal newBalance, IDbTransaction transaction)
        {
            const string sql = @"UPDATE bank_accounts
                                    SET balance = @Balance  
                                    WHERE id = @AccountId;";

            using var connection = _connectionFactory.CreateConnection();

            var rows = await connection.ExecuteAsync(
                sql,
                new
                {
                    AccountId = accountId,
                    Balance = newBalance
                },
                transaction
            );
            if (rows == 0)
                throw new InvalidOperationException("Conta não encontrada.");
        }
    }
}

