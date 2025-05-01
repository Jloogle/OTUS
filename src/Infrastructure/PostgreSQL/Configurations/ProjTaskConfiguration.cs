using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.PostgreSQL.Configurations;

public class ProjTaskConfiguration : IEntityTypeConfiguration<ProjTask>
{
    public void Configure(EntityTypeBuilder<ProjTask> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        // Связь с проектом настроена в ProjectConfiguration
    }
} 