using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.PostgreSQL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .HasMaxLength(100);
        
        builder.Property(x => x.Age);
        
        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);
        
        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .HasColumnName("email");

        builder.Property(x => x.IdTelegram)
            .IsRequired();

        builder
            .HasMany(x => x.Projects)
            .WithMany(x => x.Users)
            .UsingEntity(j => j.ToTable("ProjectUser", "OTUS"));

        builder
            .HasMany(x => x.AssignedTasks)
            .WithMany(x => x.AssignedUsers);

        builder
            .HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity(j => j.ToTable("RoleUser"));
    }
} 
