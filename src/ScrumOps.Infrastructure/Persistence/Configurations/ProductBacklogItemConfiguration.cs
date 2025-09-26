using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ProductBacklogItem entity.
/// </summary>
public class ProductBacklogItemConfiguration : IEntityTypeConfiguration<ProductBacklogItem>
{
    public void Configure(EntityTypeBuilder<ProductBacklogItem> builder)
    {
        builder.HasKey(pbi => pbi.Id);

        // Configure Id as value object
        builder.Property(pbi => pbi.Id)
            .HasConversion(
                id => id.Value,
                value => ProductBacklogItemId.From(value))
            .ValueGeneratedNever();

        // Configure ProductBacklogId as value object
        builder.Property(pbi => pbi.ProductBacklogId)
            .HasConversion(
                id => id.Value,
                value => ProductBacklogId.From(value))
            .IsRequired();

        // Configure ItemTitle value object
        builder.OwnsOne(pbi => pbi.Title, titleBuilder =>
        {
            titleBuilder.Property(t => t.Value)
                .HasColumnName("Title")
                .HasMaxLength(ItemTitle.MaxLength)
                .IsRequired();
        });

        // Configure ItemDescription value object
        builder.OwnsOne(pbi => pbi.Description, descBuilder =>
        {
            descBuilder.Property(d => d.Value)
                .HasColumnName("Description")
                .HasMaxLength(ItemDescription.MaxLength)
                .IsRequired();
        });

        // Configure Priority value object
        builder.OwnsOne(pbi => pbi.Priority, priorityBuilder =>
        {
            priorityBuilder.Property(p => p.Value)
                .HasColumnName("Priority")
                .IsRequired();
        });

        // Configure StoryPoints value object (nullable)
        builder.OwnsOne(pbi => pbi.StoryPoints, pointsBuilder =>
        {
            pointsBuilder.Property(sp => sp.Value)
                .HasColumnName("StoryPoints");
        });

        // Configure AcceptanceCriteria value object (nullable)
        builder.OwnsOne(pbi => pbi.AcceptanceCriteria, criteriaBuilder =>
        {
            criteriaBuilder.Property(ac => ac.Value)
                .HasColumnName("AcceptanceCriteria")
                .HasMaxLength(AcceptanceCriteria.MaxLength);
        });

        // Configure CreatedBy as UserName value object
        builder.OwnsOne(pbi => pbi.CreatedBy, createdByBuilder =>
        {
            createdByBuilder.Property(cb => cb.Value)
                .HasColumnName("CreatedBy")
                .HasMaxLength(UserName.MaxLength)
                .IsRequired();
        });

        // Configure enum properties
        builder.Property(pbi => pbi.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pbi => pbi.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Configure simple properties
        builder.Property(pbi => pbi.CreatedDate)
            .IsRequired();

        builder.Property(pbi => pbi.LastModifiedDate)
            .IsRequired();

        // Configure indexes
        builder.HasIndex(pbi => pbi.ProductBacklogId)
            .HasDatabaseName("IX_ProductBacklogItems_ProductBacklogId");

        builder.HasIndex("Priority")
            .HasDatabaseName("IX_ProductBacklogItems_Priority");

        builder.HasIndex(pbi => pbi.Status)
            .HasDatabaseName("IX_ProductBacklogItems_Status");

        builder.HasIndex(pbi => pbi.Type)
            .HasDatabaseName("IX_ProductBacklogItems_Type");

        builder.HasIndex(pbi => pbi.CreatedDate)
            .HasDatabaseName("IX_ProductBacklogItems_CreatedDate");

        builder.HasIndex("Title")
            .HasDatabaseName("IX_ProductBacklogItems_Title");
    }
}