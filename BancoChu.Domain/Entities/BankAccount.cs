using BancoChu.Domain.Enums;

namespace BancoChu.Domain.Entities
{
    public class BankAccount
    {
        public Guid Id { get; private set; }
        public string AccountNumber { get; private set; }
        public string Agency { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }
        public AccountStatus Status { get; private set; }
        public AccountType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private BankAccount() { }

        public static BankAccount Create(
            string accountNumber,
            string agency,
            Guid userId,
            decimal balance,
            AccountType type)
        {
            return new BankAccount
            {
                Id = Guid.NewGuid(),
                AccountNumber = accountNumber,
                Agency = agency,
                UserId = userId,
                Balance = balance,
                Status = AccountStatus.Active,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}

