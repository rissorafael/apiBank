using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;
using Dapper;


namespace BancoChu.Infrastructure.Repositories
{
    public class BankTransferRepository : IBankTransferRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BankTransferRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Guid> TransferAsync(BankTransfer transfer)
        {
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
            );
        ";

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(
                    sql,
                    transfer,
                    transaction
                );

                transaction.Commit();
                return transfer.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

