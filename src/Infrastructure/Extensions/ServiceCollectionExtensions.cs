using Domain.Repositories;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddTransient<IAdapterApplicationContext, AdapterApplicationContext>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();

            return services;
        }
    }
} 