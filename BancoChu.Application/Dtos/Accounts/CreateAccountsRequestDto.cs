using BancoChu.Domain.Enums;

namespace BancoChu.Application.Dtos.Accounts
{
    public class CreateAccountsRequestDto
    {
        public string AccountNumber { get; set; }
        public string Agency { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public AccountType Type { get; set; }
    }
}
