using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ProductBacklog aggregate root.
/// </summary>
public class ProductBacklogConfiguration : IEntityTypeConfiguration<ProductBacklog>
{
    public void Configure(EntityTypeBuilder<ProductBacklog> builder)
    {
        builder.HasKey(pb => pb.Id);

        // Configure Id as value object
        builder.Property(pb => pb.Id)
            .HasConversion(
                id => id.Value,
                value => ProductBacklogId.From(value))
            .ValueGeneratedNever();

        // Configure TeamId as value object
        builder.Property(pb => pb.TeamId)
            .HasConversion(
                id => id.Value,
                value => TeamId.From(value))
            .IsRequired();

        // Configure BacklogNotes value object
        builder.OwnsOne(pb => pb.Notes, notesBuilder =>
        {
            notesBuilder.Property(n => n.Value)
                .HasColumnName("Notes")
                .HasMaxLength(5000);
        });

        // Configure simple properties
        builder.Property(pb => pb.CreatedDate)
            .IsRequired();

        builder.Property(pb => pb.LastRefinedDate)
            .IsRequired(false);

        // Configure relationships
        builder.HasMany(pb => pb.Items)
            .WithOne()
            .HasForeignKey("ProductBacklogId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events (handled separately)
        builder.Ignore(pb => pb.DomainEvents);

        // Configure indexes
        builder.HasIndex(pb => pb.TeamId)
            .IsUnique()
            .HasDatabaseName("IX_ProductBacklogs_TeamId");

        builder.HasIndex(pb => pb.CreatedDate)
            .HasDatabaseName("IX_ProductBacklogs_CreatedDate");

        builder.HasIndex(pb => pb.LastRefinedDate)
            .HasDatabaseName("IX_ProductBacklogs_LastRefinedDate");

        // Configure table and schema
        builder.ToTable("ProductBacklogs", "ProductBacklog");
    }
}
