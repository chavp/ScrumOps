using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Task entity.
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<Domain.SprintManagement.Entities.Task>
{
    public void Configure(EntityTypeBuilder<Domain.SprintManagement.Entities.Task> builder)
    {
        builder.HasKey(t => t.Id);

        // Configure Id as value object
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => TaskId.From(value))
            .ValueGeneratedNever();

        // Configure SprintBacklogItemId as value object
        builder.Property(t => t.SprintBacklogItemId)
            .HasConversion(
                id => id.Value,
                value => SprintBacklogItemId.From(value))
            .IsRequired();

        // Configure TaskTitle value object
        builder.OwnsOne(t => t.Title, titleBuilder =>
        {
            titleBuilder.Property(tt => tt.Value)
                .HasColumnName("Title")
                .HasMaxLength(TaskTitle.MaxLength)
                .IsRequired();
        });

        // Configure TaskDescription value object
        builder.OwnsOne(t => t.Description, descBuilder =>
        {
            descBuilder.Property(td => td.Value)
                .HasColumnName("Description")
                .HasMaxLength(TaskDescription.MaxLength)
                .IsRequired();
        });

        // Configure enum properties
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Configure simple properties
        builder.Property(t => t.OriginalEstimateHours)
            .IsRequired();

        builder.Property(t => t.RemainingHours)
            .IsRequired();

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.StartedDate)
            .IsRequired(false);

        builder.Property(t => t.CompletedDate)
            .IsRequired(false);

        // Configure indexes
        builder.HasIndex(t => t.SprintBacklogItemId)
            .HasDatabaseName("IX_Tasks_SprintBacklogItemId");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tasks_Status");

        builder.HasIndex(t => t.CreatedDate)
            .HasDatabaseName("IX_Tasks_CreatedDate");

        // Configure table and schema
        builder.ToTable("Tasks", "SprintManagement");
    }
}