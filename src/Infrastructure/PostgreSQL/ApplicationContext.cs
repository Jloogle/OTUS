
using Domain.Entities;
using Infrastructure.PostgreSQL.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.PostgreSQL;

public sealed class ApplicationContext  : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<ProjTask> Tasks { get; set; }
    public DbSet<Project?> Projects { get; set; }
  

    public ApplicationContext()
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();
        
        optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
    }

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
