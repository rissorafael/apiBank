using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;
using Dapper;
using System.Data;


namespace BancoChu.Infrastructure.Repositories
{
    public class BankTransferRepository : IBankTransferRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BankTransferRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Guid> TransferAsync(BankTransfer transfer, IDbTransaction transaction)
        {
            try
            {
                if (transaction == null) throw new ArgumentNullException(nameof(transaction));
                if (transaction.Connection == null) throw new ArgumentNullException(nameof(transaction.Connection));

                const string sql = @"
            INSERT INTO bank_transfers
            (
                transfer_id,
                origin_account_id,
                destination_account_id,
                amount,
                transfer_date,
                created_at,
                status
            )
            VALUES
            (
                @TransferId,
                @OriginAccountId,
                @DestinationAccountId,
                @Amount,
                @TransferDate,
                @CreatedAt,
                @Status
            );";

                await transaction.Connection.ExecuteAsync(sql, new
                {
                    TransferId = transfer.Id,
                    OriginAccountId = transfer.OriginAccountId,
                    DestinationAccountId = transfer.DestinationAccountId,
                    Amount = transfer.Amount,
                    TransferDate = transfer.TransferDate,
                    CreatedAt = transfer.CreatedAt,
                    Status = transfer.Status.ToString()
                }, transaction);

                return transfer.Id;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<BankTransfer>> GetStatementAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                       SELECT
                           transfer_id       AS Id,
                           origin_account_id AS OriginAccountId,
                           destination_account_id AS DestinationAccountId,
                           amount            AS Amount,
                           transfer_date     AS TransferDate,
                           status            AS Status,
                           created_at        AS CreatedAt
                       FROM bank_transfers
                       WHERE
                           (origin_account_id = @AccountId
                            OR destination_account_id = @AccountId)
                         AND transfer_date >= @StartDate
                         AND transfer_date < @EndDate
                       ORDER BY transfer_date ASC;";


            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<BankTransfer>(sql, new
            {
                AccountId = accountId,
                StartDate = startDate,
                EndDate = endDate
            });
        }

    }
}

