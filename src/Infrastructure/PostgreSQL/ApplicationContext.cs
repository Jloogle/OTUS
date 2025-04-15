using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.PostgreSQL;

public sealed class ApplicationContext  : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<ProjTask> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    
    public ApplicationContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
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
}