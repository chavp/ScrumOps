using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using SprintTaskStatus = ScrumOps.Domain.SprintManagement.ValueObjects.TaskStatus;
using SprintVelocity = ScrumOps.Domain.SprintManagement.ValueObjects.Velocity;

namespace ScrumOps.Domain.Tests.SprintManagement;

/// <summary>
/// Basic domain tests for Sprint value objects and business rules.
/// Tests core Scrum framework compliance without complex entity relationships.
/// </summary>
public class SprintBusinessRulesTests
{
    [Fact]
    public void SprintGoal_Create_WithValidValue_ShouldSucceed()
    {
        // Arrange
        const string goalText = "Implement user authentication system";

        // Act
        var sprintGoal = SprintGoal.Create(goalText);

        // Assert
        Assert.NotNull(sprintGoal);
        Assert.Equal(goalText, sprintGoal.Value);
    }

    [Fact]
    public void SprintGoal_Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        const string emptyGoal = "";

        // Act & Assert
        Assert.Throws<DomainException>(() => SprintGoal.Create(emptyGoal));
    }

    [Fact]
    public void SprintGoal_Create_WithNullValue_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => SprintGoal.Create(null!));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    public void StoryPoints_Create_WithValidValues_ShouldSucceed(int points)
    {
        // Act
        var storyPoints = StoryPoints.Create(points);

        // Assert
        Assert.NotNull(storyPoints);
        Assert.Equal(points, storyPoints.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(0)]  // 0 is not valid for story points
    [InlineData(4)]  // Not in Fibonacci sequence
    [InlineData(100)]  // Too large
    public void StoryPoints_Create_WithInvalidValues_ShouldThrowException(int invalidPoints)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => StoryPoints.Create(invalidPoints));
    }

    [Fact]
    public void Capacity_Create_WithValidValue_ShouldSucceed()
    {
        // Arrange
        const int capacityValue = 80;

        // Act
        var capacity = Capacity.Create(capacityValue);

        // Assert
        Assert.NotNull(capacity);
        Assert.Equal(capacityValue, capacity.Hours); // Use Hours property instead of Value
    }

    [Theory]
    [InlineData(-5)]  // Remove 0 since it's valid for capacity
    public void Capacity_Create_WithInvalidValue_ShouldThrowException(int invalidCapacity)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => Capacity.Create(invalidCapacity));
    }

    [Fact]
    public void SprintId_New_ShouldCreateUniqueIds()
    {
        // Act
        var id1 = SprintId.New();
        var id2 = SprintId.New();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(Guid.Empty, id1.Value);
        Assert.NotEqual(Guid.Empty, id2.Value);
    }

    [Fact]
    public void TaskTitle_Create_WithValidValue_ShouldSucceed()
    {
        // Arrange
        const string title = "Implement login validation";

        // Act
        var taskTitle = TaskTitle.Create(title);

        // Assert
        Assert.NotNull(taskTitle);
        Assert.Equal(title, taskTitle.Value);
    }

    [Fact]
    public void TaskTitle_Create_WithEmptyValue_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => TaskTitle.Create(""));
    }

    [Theory]
    [InlineData("A")]  // Too short (minimum 3 characters expected)
    [InlineData("AB")] // Too short
    public void TaskTitle_Create_WithTooShortValue_ShouldThrowException(string shortTitle)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => TaskTitle.Create(shortTitle));
    }

    [Fact]
    public void SprintVelocity_Create_WithValidValue_ShouldSucceed()
    {
        // Arrange
        const int velocityValue = 25; // Use int instead of decimal

        // Act
        var velocity = SprintVelocity.Create(velocityValue);

        // Assert
        Assert.NotNull(velocity);
        Assert.Equal(velocityValue, velocity.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-25)]
    public void SprintVelocity_Create_WithNegativeValue_ShouldThrowException(int negativeVelocity) // Use int instead of decimal
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => SprintVelocity.Create(negativeVelocity));
    }

    [Fact]
    public void SprintStatus_ShouldHaveExpectedValues()
    {
        // Assert - Verify that SprintStatus enum has expected values
        Assert.True(Enum.IsDefined(typeof(SprintStatus), SprintStatus.Planning));
        Assert.True(Enum.IsDefined(typeof(SprintStatus), SprintStatus.Active));
        Assert.True(Enum.IsDefined(typeof(SprintStatus), SprintStatus.Completed));
    }

    [Fact]
    public void SprintTaskStatus_ShouldHaveExpectedValues()
    {
        // Assert - Verify that TaskStatus enum has expected values  
        Assert.True(Enum.IsDefined(typeof(SprintTaskStatus), SprintTaskStatus.ToDo)); // Use correct enum value
        Assert.True(Enum.IsDefined(typeof(SprintTaskStatus), SprintTaskStatus.InProgress));
        Assert.True(Enum.IsDefined(typeof(SprintTaskStatus), SprintTaskStatus.Done));
    }

    [Theory]
    [InlineData(2, 14)]   // 2 weeks
    [InlineData(1, 7)]    // 1 week  
    [InlineData(3, 21)]   // 3 weeks
    public void Sprint_DateCalculations_ShouldBeAccurate(int sprintLengthWeeks, int expectedDays)
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1); // Monday
        var expectedEndDate = startDate.AddDays(expectedDays);

        // Act
        var actualEndDate = startDate.AddDays(sprintLengthWeeks * 7);

        // Assert
        Assert.Equal(expectedEndDate, actualEndDate);
        Assert.Equal(expectedDays, (actualEndDate - startDate).Days);
    }

    [Fact]
    public void ProductBacklogItemId_New_ShouldCreateUniqueIds()
    {
        // Act
        var id1 = ProductBacklogItemId.New();
        var id2 = ProductBacklogItemId.New();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(Guid.Empty, id1.Value);
        Assert.NotEqual(Guid.Empty, id2.Value);
    }

    [Fact]
    public void StoryPoints_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var points1 = StoryPoints.Create(8);
        var points2 = StoryPoints.Create(8);
        var points3 = StoryPoints.Create(13);

        // Assert
        Assert.Equal(points1, points2);
        Assert.NotEqual(points1, points3);
        Assert.True(points1.Equals(points2));
        Assert.False(points1.Equals(points3));
    }

    [Fact]
    public void Capacity_Calculations_ShouldBeAccurate()
    {
        // Arrange
        var totalCapacity = Capacity.Create(100);
        var usedPoints = StoryPoints.Create(34); // Use valid Fibonacci number

        // Act
        var remainingCapacity = totalCapacity.Hours - usedPoints.Value; // Use Hours property

        // Assert
        Assert.Equal(66, remainingCapacity); // 100 - 34 = 66
        Assert.True(remainingCapacity > 0);
        Assert.True(usedPoints.Value < totalCapacity.Hours); // Use Hours property
    }
}