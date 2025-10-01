using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Tests.Builders;

/// <summary>
/// Test builder for creating ProductBacklog entities with valid test data.
/// </summary>
public class ProductBacklogBuilder
{
    private ProductBacklogId _id = ProductBacklogId.New();
    private TeamId _teamId = TeamId.New();
    private BacklogNotes _notes = BacklogNotes.Empty;

    public ProductBacklogBuilder WithId(ProductBacklogId id)
    {
        _id = id;
        return this;
    }

    public ProductBacklogBuilder WithTeamId(TeamId teamId)
    {
        _teamId = teamId;
        return this;
    }

    public ProductBacklogBuilder WithNotes(string notes)
    {
        _notes = BacklogNotes.Create(notes);
        return this;
    }

    public ProductBacklog Build()
    {
        return new ProductBacklog(_id, _teamId);
    }

    /// <summary>
    /// Creates a product backlog with random but valid data.
    /// </summary>
    public static ProductBacklog Random(TeamId? teamId = null)
    {
        return new ProductBacklogBuilder()
            .WithTeamId(teamId ?? TeamId.New())
            .WithNotes($"Test backlog created at {DateTime.UtcNow}")
            .Build();
    }
}

/// <summary>
/// Test builder for creating ProductBacklogItem entities with valid test data.
/// </summary>
public class ProductBacklogItemBuilder
{
    private ProductBacklogItemId _id = ProductBacklogItemId.New();
    private ProductBacklogId _productBacklogId = ProductBacklogId.New();
    private ItemTitle _title = ItemTitle.Create("Test Item");
    private ItemDescription _description = ItemDescription.Create("Test description");
    private BacklogItemType _type = BacklogItemType.UserStory;
    private UserName _createdBy = UserName.Create("Test User");

    public ProductBacklogItemBuilder WithId(ProductBacklogItemId id)
    {
        _id = id;
        return this;
    }

    public ProductBacklogItemBuilder WithProductBacklogId(ProductBacklogId productBacklogId)
    {
        _productBacklogId = productBacklogId;
        return this;
    }

    public ProductBacklogItemBuilder WithTitle(string title)
    {
        _title = ItemTitle.Create(title);
        return this;
    }

    public ProductBacklogItemBuilder WithDescription(string description)
    {
        _description = ItemDescription.Create(description);
        return this;
    }

    public ProductBacklogItemBuilder WithType(BacklogItemType type)
    {
        _type = type;
        return this;
    }

    public ProductBacklogItemBuilder WithCreatedBy(string createdBy)
    {
        _createdBy = UserName.Create(createdBy);
        return this;
    }

    public ProductBacklogItem Build()
    {
        return new ProductBacklogItem(_id, _productBacklogId, _title, _description, _type, _createdBy);
    }

    /// <summary>
    /// Creates a backlog item with random but valid data.
    /// </summary>
    public static ProductBacklogItem Random(ProductBacklogId? productBacklogId = null)
    {
        var random = new Random();
        var types = Enum.GetValues<BacklogItemType>();
        
        return new ProductBacklogItemBuilder()
            .WithProductBacklogId(productBacklogId ?? ProductBacklogId.New())
            .WithTitle($"Item {random.Next(1000, 9999)}")
            .WithDescription($"Test item created at {DateTime.UtcNow}")
            .WithType(types[random.Next(types.Length)])
            .WithCreatedBy($"User {random.Next(1, 100)}")
            .Build();
    }

    /// <summary>
    /// Creates multiple backlog items for a product backlog.
    /// </summary>
    public static List<ProductBacklogItem> CreateMultiple(ProductBacklogId productBacklogId, int count)
    {
        var items = new List<ProductBacklogItem>();
        for (int i = 0; i < count; i++)
        {
            items.Add(new ProductBacklogItemBuilder()
                .WithProductBacklogId(productBacklogId)
                .WithTitle($"User Story {i + 1}")
                .WithDescription($"As a user, I want feature {i + 1}")
                .WithType(BacklogItemType.UserStory)
                .WithCreatedBy("Product Owner")
                .Build());
        }
        return items;
    }
}