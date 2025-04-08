using Domain.Repositories;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            // Регистрация контекста базы данных
            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(connectionString));

            // Регистрация репозиториев
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();

            return services;
        }
    }
} 