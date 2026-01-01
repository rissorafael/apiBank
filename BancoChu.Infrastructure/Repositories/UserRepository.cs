using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;
using Dapper;


namespace BancoChu.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            var sql = @"
                SELECT 
                    id, 
                    email, 
                    password, 
                    status, 
                    created_at AS CreatedAt
                FROM users
                WHERE email = @Email
            ";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
            return result;
        }

        public async Task AddAsync(User user)
        {
            const string sql = @"
            INSERT INTO users 
            (
                id,
                email,
                password,
                status,
                created_at
            )
            VALUES 
            (
                @Id,
                @Email,
                @Password,
                @Status,
                @CreatedAt
            );";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, user);
        }
    }
}