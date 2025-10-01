using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Sprint aggregate root.
/// </summary>
public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.HasKey(s => s.Id);

        // Configure Id as value object
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SprintId.From(value))
            .ValueGeneratedNever();

        // Configure TeamId as value object
        builder.Property(s => s.TeamId)
            .HasConversion(
                id => id.Value,
                value => TeamId.From(value))
            .IsRequired();

        // Configure SprintGoal value object
        builder.OwnsOne(s => s.Goal, goalBuilder =>
        {
            goalBuilder.Property(g => g.Value)
                .HasColumnName("Goal")
                .HasMaxLength(SprintGoal.MaxLength)
                .IsRequired();
        });

        // Configure Capacity value object
        builder.OwnsOne(s => s.Capacity, capacityBuilder =>
        {
            capacityBuilder.Property(c => c.Hours)
                .HasColumnName("CapacityHours")
                .IsRequired();
        });

        // Configure ActualVelocity value object (nullable)
        builder.OwnsOne(s => s.ActualVelocity, velocityBuilder =>
        {
            velocityBuilder.Property(v => v.Value)
                .HasColumnName("ActualVelocity");
        });

        // Configure enum properties
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Configure simple properties
        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.ActualStartDate)
            .IsRequired(false);

        builder.Property(s => s.ActualEndDate)
            .IsRequired(false);

        builder.Property(s => s.CreatedDate)
            .IsRequired();

        // Configure relationships
        builder.HasMany(s => s.BacklogItems)
            .WithOne()
            .HasForeignKey("SprintId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events (handled separately)
        builder.Ignore(s => s.DomainEvents);

        // Configure indexes
        builder.HasIndex(s => s.TeamId)
            .HasDatabaseName("IX_Sprints_TeamId");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Sprints_Status");

        builder.HasIndex(s => s.StartDate)
            .HasDatabaseName("IX_Sprints_StartDate");

        builder.HasIndex(s => s.EndDate)
            .HasDatabaseName("IX_Sprints_EndDate");

        builder.HasIndex(s => s.CreatedDate)
            .HasDatabaseName("IX_Sprints_CreatedDate");

        // Compound index for team + status queries
        builder.HasIndex(s => new { s.TeamId, s.Status })
            .HasDatabaseName("IX_Sprints_TeamId_Status");

        // Configure table and schema
        builder.ToTable("Sprints", "SprintManagement");
    }
}