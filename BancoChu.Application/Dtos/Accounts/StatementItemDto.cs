namespace BancoChu.Application.Dtos.Accounts
{
    public class StatementItemDto
    {
        public Guid TransferId { get; set; }
        public Guid OriginAccountId { get; set; }
        public Guid DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string Status { get; set; }
    }
}
