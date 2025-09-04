
using Domain.Entities;
using Infrastructure.PostgreSQL.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.PostgreSQL;

/// <summary>
/// DbContext Entity Framework Core с провайдером PostgreSQL и схемой OTUS.
/// </summary>
public sealed class ApplicationContext  : DbContext
{
    /// <summary>Набор пользователей.</summary>
    public DbSet<User> Users { get; set; }
    /// <summary>Набор уведомлений.</summary>
    public DbSet<Notification> Notifications { get; set; }
    /// <summary>Набор ролей.</summary>
    public DbSet<Role> Roles { get; set; }
    /// <summary>Набор задач.</summary>
    public DbSet<ProjTask> Tasks { get; set; }
    /// <summary>Набор проектов.</summary>
    public DbSet<Project?> Projects { get; set; }
  

    public ApplicationContext()
    {
        // Do not run migrations here; it breaks design-time tools
    }
    /// <summary>
    /// Настраивает DbContext, получая строку подключения из appsettings или используя значения по умолчанию.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var cs = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(cs))
        {
            cs = "Host=localhost;Port=5432;Database=postgres_db;Username=postgres;Password=postgres";
        }
        optionsBuilder.UseNpgsql(cs);
    }

    /// <summary>
    /// Применяет конфигурации сущностей и сопоставления связей.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("OTUS");
        
        // Применяем все конфигурации
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjTaskConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        
        // Настройка связи многие-ко-многим между User и ProjTask
        modelBuilder.Entity<User>()
            .HasMany(u => u.AssignedTasks)
            .WithMany(t => t.AssignedUsers)
            .UsingEntity<Dictionary<string, object>>(
                "UserTasks", // Имя промежуточной таблицы
                j => j
                    .HasOne<ProjTask>()
                    .WithMany()
                    .HasForeignKey("TaskId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "TaskId");
                    j.ToTable("UserTasks", "OTUS");
                    j.HasIndex("UserId");
                    j.HasIndex("TaskId");
                });
    }
}
