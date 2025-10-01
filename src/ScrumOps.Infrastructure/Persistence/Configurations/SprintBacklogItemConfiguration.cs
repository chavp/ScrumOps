using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for SprintBacklogItem entity.
/// </summary>
public class SprintBacklogItemConfiguration : IEntityTypeConfiguration<SprintBacklogItem>
{
    public void Configure(EntityTypeBuilder<SprintBacklogItem> builder)
    {
        builder.HasKey(sbi => sbi.Id);

        // Configure Id as value object
        builder.Property(sbi => sbi.Id)
            .HasConversion(
                id => id.Value,
                value => SprintBacklogItemId.From(value))
            .ValueGeneratedNever();

        // Configure SprintId as value object
        builder.Property(sbi => sbi.SprintId)
            .HasConversion(
                id => id.Value,
                value => SprintId.From(value))
            .IsRequired();

        // Configure ProductBacklogItemId as value object
        builder.Property(sbi => sbi.ProductBacklogItemId)
            .HasConversion(
                id => id.Value,
                value => ScrumOps.Domain.SprintManagement.ValueObjects.ProductBacklogItemId.From(value))
            .IsRequired();

        // Configure StoryPoints value object
        builder.OwnsOne(sbi => sbi.StoryPoints, pointsBuilder =>
        {
            pointsBuilder.Property(sp => sp.Value)
                .HasColumnName("StoryPoints")
                .IsRequired();
        });

        // Configure simple properties
        builder.Property(sbi => sbi.RemainingWork)
            .IsRequired();

        builder.Property(sbi => sbi.IsCompleted)
            .IsRequired();

        builder.Property(sbi => sbi.CompletedDate)
            .IsRequired(false);

        // Configure relationships
        builder.HasMany(sbi => sbi.Tasks)
            .WithOne()
            .HasForeignKey("SprintBacklogItemId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed properties
        builder.Ignore(sbi => sbi.IsCompleted);

        // Configure indexes
        builder.HasIndex(sbi => sbi.SprintId)
            .HasDatabaseName("IX_SprintBacklogItems_SprintId");

        builder.HasIndex(sbi => sbi.ProductBacklogItemId)
            .IsUnique()
            .HasDatabaseName("IX_SprintBacklogItems_ProductBacklogItemId");

        builder.HasIndex(sbi => sbi.CompletedDate)
            .HasDatabaseName("IX_SprintBacklogItems_CompletedDate");

        // Compound index for sprint + completion status
        builder.HasIndex(sbi => new { sbi.SprintId, sbi.CompletedDate })
            .HasDatabaseName("IX_SprintBacklogItems_SprintId_CompletedDate");

        // Configure table and schema
        builder.ToTable("SprintBacklogItems", "SprintManagement");
    }
}