using Guardian.Domain.User.Repositories;
using Guardian.Infrastructure.Data.Contexts;
using Guardian.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guardian.Infrastructure.Data
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddDataInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                   connectionString,
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                );
            });

            #region Repositories

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            #endregion

            return services;
        }
    }
}