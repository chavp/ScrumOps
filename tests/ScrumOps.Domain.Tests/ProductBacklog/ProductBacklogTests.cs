using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.ProductBacklog.Events;
using ProductBacklogEntity = ScrumOps.Domain.ProductBacklog.Entities.ProductBacklog;

namespace ScrumOps.Domain.Tests.ProductBacklog;

/// <summary>
/// Domain tests for ProductBacklog aggregate root.
/// These tests MUST FAIL initially to follow TDD principles.
/// </summary>
public class ProductBacklogTests
{
    [Fact]
    public void ProductBacklog_Create_ShouldInitializeCorrectly()
    {
        // Arrange
        var backlogId = ProductBacklogId.New();
        var teamId = TeamId.New();

        // Act
        var backlog = new ProductBacklogEntity(backlogId, teamId);

        // Assert
        Assert.Equal(backlogId, backlog.Id);
        Assert.Equal(teamId, backlog.TeamId);
        Assert.Empty(backlog.Items);
        Assert.Equal(BacklogNotes.Empty, backlog.Notes);
    }

    [Fact]
    public void ProductBacklog_AddItem_ShouldAddItemWithCorrectPriority()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var item = CreateValidBacklogItem(backlog.Id);

        // Act
        backlog.AddItem(item);

        // Assert
        Assert.Single(backlog.Items);
        Assert.Contains(item, backlog.Items);
        Assert.Equal(Priority.Create(1), item.Priority);
        Assert.Contains(backlog.DomainEvents, e => e is BacklogItemAddedEvent);
    }

    [Fact]
    public void ProductBacklog_AddMultipleItems_ShouldAssignIncrementalPriorities()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var item1 = CreateValidBacklogItem(backlog.Id, "Item 1");
        var item2 = CreateValidBacklogItem(backlog.Id, "Item 2");
        var item3 = CreateValidBacklogItem(backlog.Id, "Item 3");

        // Act
        backlog.AddItem(item1);
        backlog.AddItem(item2);
        backlog.AddItem(item3);

        // Assert
        Assert.Equal(3, backlog.Items.Count);
        Assert.Equal(Priority.Create(1), item1.Priority);
        Assert.Equal(Priority.Create(2), item2.Priority);
        Assert.Equal(Priority.Create(3), item3.Priority);
    }

    [Fact]
    public void ProductBacklog_AddItem_WithDuplicateTitle_ShouldThrowDomainException()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var title = ItemTitle.Create("Duplicate Item");
        var item1 = CreateValidBacklogItem(backlog.Id, title);
        var item2 = CreateValidBacklogItem(backlog.Id, title);
        
        backlog.AddItem(item1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => backlog.AddItem(item2));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void ProductBacklog_ReorderItems_ShouldUpdatePriorities()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var item1 = CreateValidBacklogItem(backlog.Id, "Item 1");
        var item2 = CreateValidBacklogItem(backlog.Id, "Item 2");
        
        backlog.AddItem(item1);
        backlog.AddItem(item2);

        var reorderData = new[]
        {
            (item2.Id, 1),
            (item1.Id, 2)
        };

        // Act
        backlog.ReorderItems(reorderData);

        // Assert
        Assert.Equal(Priority.Create(1), item2.Priority);
        Assert.Equal(Priority.Create(2), item1.Priority);
        Assert.Contains(backlog.DomainEvents, e => e is BacklogReorderedEvent);
    }

    [Fact]
    public void ProductBacklog_MarkAsRefined_ShouldUpdateRefinementDetails()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var refinedDate = DateTime.UtcNow;
        var notes = BacklogNotes.Create("Refined with team input");

        // Act
        backlog.MarkAsRefined(refinedDate, notes);

        // Assert
        Assert.Equal(refinedDate, backlog.LastRefinedDate);
        Assert.Equal(notes, backlog.Notes);
        Assert.Contains(backlog.DomainEvents, e => e is BacklogRefinedEvent);
    }

    [Fact]
    public void ProductBacklogItem_EstimateStoryPoints_ShouldUpdateStatusToReady()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var item = CreateValidBacklogItem(backlog.Id);
        var storyPoints = StoryPoints.Create(5);

        // Act
        item.EstimateStoryPoints(storyPoints);

        // Assert
        Assert.Equal(storyPoints, item.StoryPoints);
        Assert.Equal(BacklogItemStatus.Ready, item.Status);
    }

    [Fact]
    public void ProductBacklogItem_MarkAsInProgress_FromNewStatus_ShouldThrowDomainException()
    {
        // Arrange
        var backlog = CreateValidProductBacklog();
        var item = CreateValidBacklogItem(backlog.Id);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.MarkAsInProgress());
        Assert.Contains("Only ready items", exception.Message);
    }

    private static ProductBacklogEntity CreateValidProductBacklog()
    {
        return new ProductBacklogEntity(ProductBacklogId.New(), TeamId.New());
    }

    private static ProductBacklogItem CreateValidBacklogItem(ProductBacklogId backlogId, string title = "Test Item")
    {
        return new ProductBacklogItem(
            ProductBacklogItemId.New(),
            backlogId,
            ItemTitle.Create(title),
            ItemDescription.Create("Test description"),
            BacklogItemType.UserStory,
            UserName.Create("Product Owner")
        );
    }

    private static ProductBacklogItem CreateValidBacklogItem(ProductBacklogId backlogId, ItemTitle title)
    {
        return new ProductBacklogItem(
            ProductBacklogItemId.New(),
            backlogId,
            title,
            ItemDescription.Create("Test description"),
            BacklogItemType.UserStory,
            UserName.Create("Product Owner")
        );
    }
}