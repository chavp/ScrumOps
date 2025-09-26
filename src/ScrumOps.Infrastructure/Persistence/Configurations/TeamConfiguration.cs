using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Team aggregate root.
/// </summary>
public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        // Configure Id as value object
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => TeamId.From(value))
            .ValueGeneratedNever();

        // Configure TeamName value object
        builder.OwnsOne(t => t.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(TeamName.MaxLength)
                .IsRequired();
        });

        // Configure TeamDescription value object
        builder.OwnsOne(t => t.Description, descBuilder =>
        {
            descBuilder.Property(d => d.Value)
                .HasColumnName("Description")
                .HasMaxLength(TeamDescription.MaxLength);
        });

        // Configure SprintLength value object
        builder.OwnsOne(t => t.DefaultSprintLength, lengthBuilder =>
        {
            lengthBuilder.Property(l => l.Weeks)
                .HasColumnName("DefaultSprintLengthWeeks");
        });

        // Configure simple properties
        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.LastModifiedDate)
            .IsRequired();

        // Configure relationships
        builder.HasMany(t => t.Members)
            .WithOne()
            .HasForeignKey("TeamId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ScrumMasterId as nullable foreign key
        builder.Property<Guid?>("ScrumMasterId")
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey("ScrumMasterId")
            .OnDelete(DeleteBehavior.SetNull);

        // Configure ProductOwnerId as nullable foreign key  
        builder.Property<Guid?>("ProductOwnerId")
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey("ProductOwnerId")
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore domain events (handled separately)
        builder.Ignore(t => t.DomainEvents);

        // Configure indexes
        builder.HasIndex("Name")
            .IsUnique()
            .HasDatabaseName("IX_Teams_Name");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_Teams_IsActive");

        builder.HasIndex(t => t.CreatedDate)
            .HasDatabaseName("IX_Teams_CreatedDate");
    }
}