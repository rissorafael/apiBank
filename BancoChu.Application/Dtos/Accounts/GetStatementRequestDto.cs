namespace BancoChu.Application.Dtos.Accounts
{
    public class GetStatementRequestDto
    {
        public Guid AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
