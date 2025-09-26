using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // Configure Id as value object
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .ValueGeneratedNever();

        // Configure UserName value object
        builder.OwnsOne(u => u.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(UserName.MaxLength)
                .IsRequired();
        });

        // Configure Email value object
        builder.OwnsOne(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(Email.MaxLength)
                .IsRequired();
        });

        // Configure ScrumRole value object collection
        builder.OwnsMany(u => u.Roles, rolesBuilder =>
        {
            rolesBuilder.WithOwner().HasForeignKey("UserId");
            rolesBuilder.Property<int>("Id").ValueGeneratedOnAdd();
            rolesBuilder.HasKey("Id");
            
            rolesBuilder.Property(r => r.Value)
                .HasColumnName("Role")
                .HasConversion<string>()
                .IsRequired();
        });

        // Configure simple properties
        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.LastModifiedDate)
            .IsRequired();

        // Configure indexes
        builder.HasIndex("Email")
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex("Name")
            .HasDatabaseName("IX_Users_Name");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasIndex(u => u.CreatedDate)
            .HasDatabaseName("IX_Users_CreatedDate");
    }
}