namespace BancoChu.Application.Dtos.Accounts
{
    public class TransferRequestDto
    {
        public Guid DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
