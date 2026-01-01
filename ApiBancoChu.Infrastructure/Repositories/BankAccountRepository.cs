using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;
using Dapper;


namespace BancoChu.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BankAccountRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
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
    }
}
