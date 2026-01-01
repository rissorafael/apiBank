using BancoChu.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;


namespace BancoChu.Infrastructure.Connection
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }
    }
}

