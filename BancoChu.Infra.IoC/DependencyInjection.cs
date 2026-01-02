using ApiBancoChu.Infrastructure.ExternalServices;
using BancoChu.Application;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Interfaces;
using BancoChu.Infrastructure.Connection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BancoChu.Infra.IoC
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBrasilApiApplication, BrasilApiApplication>();
            services.AddScoped<IBrasilApiService, BrasilApiService>();
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IAccountsApplication, AccountsApplication>();
            services.AddScoped<IAccountsRepository, Infrastructure.Repositories.AccountsRepository>();
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<IUserRepository, Infrastructure.Repositories.UserRepository>();
            services.AddScoped<IUsersApplication, UsersApplication>();
            services.AddScoped<IBankTransferRepository, Infrastructure.Repositories.BankTransferRepository>();
            services.AddScoped<IBusinessDayApplication, BusinessDayApplication>();
        }
    }
}
