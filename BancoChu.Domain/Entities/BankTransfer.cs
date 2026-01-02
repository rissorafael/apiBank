using BancoChu.Domain.Enums;

namespace BancoChu.Domain.Entities
{
    public class BankTransfer
    {
        public Guid Id { get; private set; }
        public Guid OriginAccountId { get; private set; }
        public Guid DestinationAccountId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime TransferDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TransferStatus Status { get; private set; }

        private BankTransfer() { }

        public static BankTransfer Create(
            Guid originAccountId,
            Guid destinationAccountId,
            decimal amount)
        {
            return new BankTransfer
            {
                Id = Guid.NewGuid(),
                OriginAccountId = originAccountId,
                DestinationAccountId = destinationAccountId,
                Amount = amount,
                TransferDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Status = TransferStatus.Completed
            };
        }
    }
}
