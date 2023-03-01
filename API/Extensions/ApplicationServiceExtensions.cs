using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Repositories;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("Default"));
            });

            // Repositories
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddAutoMapper(typeof(Program).Assembly);

            return services;
        }
    }
}