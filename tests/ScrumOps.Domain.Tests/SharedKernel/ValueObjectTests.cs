using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.Tests.SharedKernel;

/// <summary>
/// Tests for shared value objects across all bounded contexts.
/// These tests MUST FAIL initially to follow TDD principles.
/// </summary>
public class ValueObjectTests
{
    #region Email Tests

    [Fact]
    public void Email_Create_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var emailString = "test@example.com";

        // Act
        var email = Email.Create(emailString);

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Email_Create_WithInvalidEmail_ShouldThrowDomainException()
    {
        // Arrange
        var invalidEmail = "invalid-email";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Email.Create(invalidEmail));
        Assert.Contains("Invalid email format", exception.Message);
    }

    [Fact]
    public void Email_Create_WithEmptyString_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Email.Create(""));
        Assert.Contains("Email cannot be empty", exception.Message);
    }

    [Fact]
    public void Email_Create_ShouldNormalizeToLowerCase()
    {
        // Arrange
        var emailString = "TEST@EXAMPLE.COM";

        // Act
        var email = Email.Create(emailString);

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Theory]
    [InlineData("test@example.com", "test@example.com", true)]
    [InlineData("test@example.com", "different@example.com", false)]
    [InlineData("TEST@EXAMPLE.COM", "test@example.com", true)]
    public void Email_Equality_ShouldWorkCorrectly(string email1, string email2, bool expectedEqual)
    {
        // Arrange
        var emailObj1 = Email.Create(email1);
        var emailObj2 = Email.Create(email2);

        // Act & Assert
        Assert.Equal(expectedEqual, emailObj1.Equals(emailObj2));
        Assert.Equal(expectedEqual, emailObj1 == emailObj2);
        Assert.Equal(!expectedEqual, emailObj1 != emailObj2);
    }

    #endregion

    #region TeamName Tests

    [Fact]
    public void TeamName_Create_WithValidName_ShouldSucceed()
    {
        // Arrange
        var name = "Alpha Team";

        // Act
        var teamName = TeamName.Create(name);

        // Assert
        Assert.Equal("Alpha Team", teamName.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void TeamName_Create_WithInvalidName_ShouldThrowDomainException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => TeamName.Create(invalidName));
        Assert.Contains("Team name cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("AB")] // Too short
    [InlineData("This is a very long team name that exceeds the maximum allowed length")] // Too long
    public void TeamName_Create_WithInvalidLength_ShouldThrowDomainException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => TeamName.Create(invalidName));
        Assert.Contains("must be between 3 and 50 characters", exception.Message);
    }

    [Fact]
    public void TeamName_Create_ShouldTrimWhitespace()
    {
        // Arrange
        var nameWithWhitespace = "  Alpha Team  ";

        // Act
        var teamName = TeamName.Create(nameWithWhitespace);

        // Assert
        Assert.Equal("Alpha Team", teamName.Value);
    }

    #endregion

    #region ScrumRole Tests

    [Fact]
    public void ScrumRole_ProductOwner_ShouldBeSingletonRole()
    {
        // Arrange
        var productOwner = ScrumRole.ProductOwner;

        // Act & Assert
        Assert.True(productOwner.IsSingletonRole());
        Assert.Equal("Product Owner", productOwner.Name);
    }

    [Fact]
    public void ScrumRole_ScrumMaster_ShouldBeSingletonRole()
    {
        // Arrange
        var scrumMaster = ScrumRole.ScrumMaster;

        // Act & Assert
        Assert.True(scrumMaster.IsSingletonRole());
        Assert.Equal("Scrum Master", scrumMaster.Name);
    }

    [Fact]
    public void ScrumRole_Developer_ShouldNotBeSingletonRole()
    {
        // Arrange
        var developer = ScrumRole.Developer;

        // Act & Assert
        Assert.False(developer.IsSingletonRole());
        Assert.Equal("Developer", developer.Name);
    }

    #endregion

    #region SprintLength Tests

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void SprintLength_Create_WithValidWeeks_ShouldSucceed(int weeks)
    {
        // Act
        var sprintLength = SprintLength.Create(weeks);

        // Assert
        Assert.Equal(weeks, sprintLength.Weeks);
        Assert.Equal(TimeSpan.FromDays(weeks * 7), sprintLength.Duration);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void SprintLength_Create_WithInvalidWeeks_ShouldThrowDomainException(int invalidWeeks)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => SprintLength.Create(invalidWeeks));
        Assert.Contains("must be between 1 and 4 weeks", exception.Message);
    }

    #endregion

    #region StoryPoints Tests

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    public void StoryPoints_Create_WithValidPoints_ShouldSucceed(int points)
    {
        // Act
        var storyPoints = StoryPoints.Create(points);

        // Assert
        Assert.Equal(points, storyPoints.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(35)]
    public void StoryPoints_Create_WithInvalidPoints_ShouldThrowDomainException(int invalidPoints)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => StoryPoints.Create(invalidPoints));
        Assert.Contains("Story points must be one of", exception.Message);
    }

    #endregion

    #region Priority Tests

    [Fact]
    public void Priority_Create_WithValidPriority_ShouldSucceed()
    {
        // Arrange
        var priorityValue = 1;

        // Act
        var priority = Priority.Create(priorityValue);

        // Assert
        Assert.Equal(priorityValue, priority.Value);
    }

    [Fact]
    public void Priority_Create_WithNegativePriority_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Priority.Create(-1));
        Assert.Contains("Priority cannot be negative", exception.Message);
    }

    [Fact]
    public void Priority_Unassigned_ShouldHaveValueZero()
    {
        // Act
        var unassigned = Priority.Unassigned;

        // Assert
        Assert.Equal(0, unassigned.Value);
    }

    #endregion

    #region AcceptanceCriteria Tests

    [Fact]
    public void AcceptanceCriteria_Create_WithValidCriteria_ShouldSucceed()
    {
        // Arrange
        var criteria = "Given a user, when they login, then they should see the dashboard";

        // Act
        var acceptanceCriteria = AcceptanceCriteria.Create(criteria);

        // Assert
        Assert.Equal(criteria, acceptanceCriteria.Value);
        Assert.False(acceptanceCriteria.IsEmpty);
    }

    [Fact]
    public void AcceptanceCriteria_Create_WithEmptyString_ShouldCreateEmpty()
    {
        // Act
        var acceptanceCriteria = AcceptanceCriteria.Create("");

        // Assert
        Assert.True(acceptanceCriteria.IsEmpty);
        Assert.Equal(string.Empty, acceptanceCriteria.Value);
    }

    [Fact]
    public void AcceptanceCriteria_Create_WithTooLongText_ShouldThrowDomainException()
    {
        // Arrange
        var longCriteria = new string('x', 5001);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => AcceptanceCriteria.Create(longCriteria));
        Assert.Contains("cannot exceed 5000 characters", exception.Message);
    }

    [Fact]
    public void AcceptanceCriteria_Empty_ShouldBeEmpty()
    {
        // Act
        var empty = AcceptanceCriteria.Empty;

        // Assert
        Assert.True(empty.IsEmpty);
        Assert.Equal(string.Empty, empty.Value);
    }

    #endregion
}