
namespace BancoChu.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }        
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Status { get; set; } = 1;     // 1 = ativo, 2 = bloqueado
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}