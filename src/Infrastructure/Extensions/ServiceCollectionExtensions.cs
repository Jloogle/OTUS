using Domain.Repositories;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repository;
using Infrastructure.Redis.Repository;
using Microsoft.Extensions.DependencyInjection;
using Domain.Services;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Регистрация контекста базы данных
            //services.AddDbContext<ApplicationContext>(options =>
            //    options.UseNpgsql(connectionString));

            // Регистрация репозиториев
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProjectRepository, ProjectRepository>();
            services.AddTransient<IAdapterApplicationContext, AdapterApplicationContext>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<IAdapterMultiplexer, AdapterMultiplexer>();
            services.AddTransient<IRadisRepository, RadisRepositoty>();
            services.AddSingleton<IInviteStore, InviteStore>();
            services.AddTransient<IRoleRepository, RoleRepository>();



            return services;
        }
    }
} 
