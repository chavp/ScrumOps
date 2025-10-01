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

        // Configure TeamId as value object
        builder.Property(u => u.TeamId)
            .HasConversion(
                id => id.Value,
                value => TeamId.From(value))
            .IsRequired();

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
            
            // Add index on the owned property
            emailBuilder.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
        });

        // Configure ScrumRole value object
        builder.OwnsOne(u => u.Role, roleBuilder =>
        {
            roleBuilder.Property(r => r.Name)
                .HasColumnName("Role")
                .HasMaxLength(50)
                .IsRequired();
            
            roleBuilder.Property(r => r.IsSingleton)
                .HasColumnName("RoleIsSingleton")
                .IsRequired();
        });

        // Configure simple properties
        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.LastLoginDate)
            .IsRequired(false);

        builder.Property(u => u.IsActive)
            .IsRequired();

        // Configure indexes
        builder.HasIndex(u => u.TeamId)
            .HasDatabaseName("IX_Users_TeamId");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        // Configure table and schema
        builder.ToTable("Users", "TeamManagement");
    }
}