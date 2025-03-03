using Application.Dto;
using Microsoft.EntityFrameworkCore;

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
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
    }
}