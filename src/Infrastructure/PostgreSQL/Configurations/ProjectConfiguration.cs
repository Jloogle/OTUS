using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.PostgreSQL.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Deadline)
            .IsRequired();

        // Связь с пользователями настроена в UserConfiguration (устаревшая маппинг)

        builder
            .HasMany(x => x.ProjTasks)
            .WithOne(x => x.Project);

        // Новая связь через ProjectMember с ролями
        builder
            .HasMany(x => x.Members)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId);
    }
} 
