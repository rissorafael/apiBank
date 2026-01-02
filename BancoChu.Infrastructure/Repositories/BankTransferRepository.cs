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

            await transaction.Connection.ExecuteAsync(sql, transfer, transaction);

            return transfer.Id;
        }
    }
}

