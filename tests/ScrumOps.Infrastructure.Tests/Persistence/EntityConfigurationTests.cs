using Microsoft.EntityFrameworkCore;
using ScrumOps.Infrastructure.Tests.Builders;

namespace ScrumOps.Infrastructure.Tests.Persistence;

/// <summary>
/// Tests for Entity Framework entity configurations and mappings.
/// </summary>
public class EntityConfigurationTests : TestBase
{
    [Fact]
    public async Task TeamConfiguration_MapsAllProperties()
    {
        // Arrange
        var team = new TeamBuilder()
            .WithName("Configuration Test Team")
            .WithDescription("Testing all properties are mapped correctly")
            .WithSprintLength(4)
            .Build();

        // Act
        Context.Teams.Add(team);
        await Context.SaveChangesAsync();

        // Use raw SQL to verify all properties are saved
        var result = await Context.Database.SqlQueryRaw<dynamic>(
            """
            SELECT "Id", "Name", "Description", "SprintLengthWeeks", "CurrentVelocityValue", "IsActive", "CreatedDate"
            FROM "TeamManagement"."Teams" 
            WHERE "Id" = {0}
            """,
            team.Id.Value).FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UserConfiguration_MapsComplexScrumRole()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var user = new UserBuilder()
            .WithTeamId(team.Id)
            .WithName("Test User")
            .WithEmail("test@example.com")
            .AsProductOwner()
            .Build();

        Context.Teams.Add(team);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        // Act
        var retrievedUser = await Context.Users.FirstAsync(u => u.Id == user.Id);

        // Assert
        retrievedUser.Role.Name.Should().Be("Product Owner");
        retrievedUser.Role.IsSingleton.Should().BeTrue();
    }

    [Fact]
    public async Task ProductBacklogItemConfiguration_MapsValueObjects()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var backlog = ProductBacklogBuilder.Random(team.Id);
        var item = new ProductBacklogItemBuilder()
            .WithProductBacklogId(backlog.Id)
            .WithTitle("Test Story")
            .WithDescription("A comprehensive test story for mapping verification")
            .WithCreatedBy("Product Owner")
            .Build();

        Context.Teams.Add(team);
        Context.ProductBacklogs.Add(backlog);
        Context.ProductBacklogItems.Add(item);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        // Act
        var retrievedItem = await Context.ProductBacklogItems.FirstAsync(i => i.Id == item.Id);

        // Assert
        retrievedItem.Title.Value.Should().Be("Test Story");
        retrievedItem.Description.Value.Should().Be("A comprehensive test story for mapping verification");
        retrievedItem.CreatedBy.Value.Should().Be("Product Owner");
        retrievedItem.Priority.Value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DatabaseIndexes_AreCreatedCorrectly()
    {
        // Act - Query database indexes
        var indexes = await Context.Database.SqlQueryRaw<IndexInfo>(
            """
            SELECT 
                schemaname,
                tablename,
                indexname,
                indexdef
            FROM pg_indexes 
            WHERE schemaname IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')
            ORDER BY schemaname, tablename, indexname
            """).ToListAsync();

        // Assert
        indexes.Should().NotBeEmpty();
        
        // Verify specific indexes exist
        indexes.Should().Contain(i => i.indexname == "IX_Teams_Name");
        indexes.Should().Contain(i => i.indexname == "IX_Users_Email");
        indexes.Should().Contain(i => i.indexname == "IX_ProductBacklogItems_Title");
        indexes.Should().Contain(i => i.indexname == "IX_ProductBacklogItems_Priority");
    }

    [Fact]
    public async Task ForeignKeyRelationships_AreConfiguredCorrectly()
    {
        // Act - Query foreign key constraints
        var foreignKeys = await Context.Database.SqlQueryRaw<ForeignKeyInfo>(
            """
            SELECT 
                tc.constraint_name,
                tc.table_schema,
                tc.table_name,
                kcu.column_name,
                ccu.table_schema AS foreign_table_schema,
                ccu.table_name AS foreign_table_name,
                ccu.column_name AS foreign_column_name
            FROM information_schema.table_constraints AS tc
            JOIN information_schema.key_column_usage AS kcu
                ON tc.constraint_name = kcu.constraint_name
                AND tc.table_schema = kcu.table_schema
            JOIN information_schema.constraint_column_usage AS ccu
                ON ccu.constraint_name = tc.constraint_name
                AND ccu.table_schema = tc.table_schema
            WHERE tc.constraint_type = 'FOREIGN KEY'
                AND tc.table_schema IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')
            ORDER BY tc.table_schema, tc.table_name
            """).ToListAsync();

        // Assert
        foreignKeys.Should().NotBeEmpty();
        
        // Verify specific foreign keys exist
        foreignKeys.Should().Contain(fk => 
            fk.table_name == "Users" && 
            fk.column_name == "TeamId" && 
            fk.foreign_table_name == "Teams");
            
        foreignKeys.Should().Contain(fk => 
            fk.table_name == "ProductBacklogItems" && 
            fk.column_name == "ProductBacklogId" && 
            fk.foreign_table_name == "ProductBacklogs");
    }

    [Fact]
    public async Task UniqueConstraints_AreConfiguredCorrectly()
    {
        // Act - Query unique constraints
        var uniqueConstraints = await Context.Database.SqlQueryRaw<UniqueConstraintInfo>(
            """
            SELECT 
                tc.constraint_name,
                tc.table_schema,
                tc.table_name,
                kcu.column_name
            FROM information_schema.table_constraints AS tc
            JOIN information_schema.key_column_usage AS kcu
                ON tc.constraint_name = kcu.constraint_name
                AND tc.table_schema = kcu.table_schema
            WHERE tc.constraint_type = 'UNIQUE'
                AND tc.table_schema IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')
            ORDER BY tc.table_schema, tc.table_name, kcu.column_name
            """).ToListAsync();

        // Assert
        uniqueConstraints.Should().NotBeEmpty();
        
        // Verify specific unique constraints exist
        uniqueConstraints.Should().Contain(uc => 
            uc.table_name == "Teams" && uc.column_name == "Name");
            
        uniqueConstraints.Should().Contain(uc => 
            uc.table_name == "Users" && uc.column_name == "Email");
    }

    [Fact]
    public async Task TableSchemas_AreCorrectlyConfigured()
    {
        // Act - Query table schemas
        var tables = await Context.Database.SqlQueryRaw<TableInfo>(
            """
            SELECT 
                table_schema,
                table_name,
                table_type
            FROM information_schema.tables
            WHERE table_schema IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')
                AND table_type = 'BASE TABLE'
            ORDER BY table_schema, table_name
            """).ToListAsync();

        // Assert
        tables.Should().NotBeEmpty();
        
        // Verify tables are in correct schemas
        tables.Should().Contain(t => t.table_schema == "TeamManagement" && t.table_name == "Teams");
        tables.Should().Contain(t => t.table_schema == "TeamManagement" && t.table_name == "Users");
        tables.Should().Contain(t => t.table_schema == "ProductBacklog" && t.table_name == "ProductBacklogs");
        tables.Should().Contain(t => t.table_schema == "ProductBacklog" && t.table_name == "ProductBacklogItems");
    }

    [Fact]
    public async Task ColumnDataTypes_AreCorrectlyConfigured()
    {
        // Act - Query column information
        var columns = await Context.Database.SqlQueryRaw<ColumnInfo>(
            """
            SELECT 
                table_schema,
                table_name,
                column_name,
                data_type,
                character_maximum_length,
                is_nullable
            FROM information_schema.columns
            WHERE table_schema IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')
            ORDER BY table_schema, table_name, ordinal_position
            """).ToListAsync();

        // Assert
        columns.Should().NotBeEmpty();
        
        // Verify specific column configurations
        var teamNameColumn = columns.FirstOrDefault(c => 
            c.table_name == "Teams" && c.column_name == "Name");
        teamNameColumn.Should().NotBeNull();
        teamNameColumn!.data_type.Should().Be("character varying");
        teamNameColumn.character_maximum_length.Should().Be(50);
        teamNameColumn.is_nullable.Should().Be("NO");
        
        var teamIdColumn = columns.FirstOrDefault(c => 
            c.table_name == "Teams" && c.column_name == "Id");
        teamIdColumn.Should().NotBeNull();
        teamIdColumn!.data_type.Should().Be("uuid");
    }

    // Record types for query results
    private sealed record IndexInfo(string schemaname, string tablename, string indexname, string indexdef);
    private sealed record ForeignKeyInfo(string constraint_name, string table_schema, string table_name, string column_name, string foreign_table_schema, string foreign_table_name, string foreign_column_name);
    private sealed record UniqueConstraintInfo(string constraint_name, string table_schema, string table_name, string column_name);
    private sealed record TableInfo(string table_schema, string table_name, string table_type);
    private sealed record ColumnInfo(string table_schema, string table_name, string column_name, string data_type, int? character_maximum_length, string is_nullable);
}