using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ScrumOps.Shared.Contracts.Teams;
using ScrumOps.Web.Components.TeamManagement;
using ScrumOps.Web.Services;

namespace ScrumOps.Web.Tests.Components;

/// <summary>
/// Unit tests for TeamListComponent using bUnit.
/// Tests component rendering, user interactions, and service integration.
/// </summary>
public class TeamListComponentTests : TestContext
{
    private readonly Mock<ITeamService> _teamServiceMock;
    private readonly Mock<ILogger<TeamListComponent>> _loggerMock;

    public TeamListComponentTests()
    {
        _teamServiceMock = new Mock<ITeamService>();
        _loggerMock = new Mock<ILogger<TeamListComponent>>();
        
        Services.AddSingleton(_teamServiceMock.Object);
        Services.AddSingleton(_loggerMock.Object);
    }

    [Fact]
    public void TeamListComponent_WithNoTeams_ShouldShowEmptyState()
    {
        // Arrange
        var emptyResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>(),
            TotalCount = 0
        };

        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ReturnsAsync(emptyResponse);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert
        Assert.Contains("No teams found", component.Markup);
        Assert.Contains("Create First Team", component.Markup);
    }

    [Fact]
    public void TeamListComponent_WithTeams_ShouldDisplayTeamCards()
    {
        // Arrange
        var teamsResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>
            {
                new()
                {
                    Id = 1,
                    Name = "Alpha Team",
                    Description = "Frontend development team",
                    SprintLengthWeeks = 2,
                    Velocity = 25.5m,
                    MemberCount = 5,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow.AddDays(-30)
                },
                new()
                {
                    Id = 2,
                    Name = "Beta Team",
                    Description = "Backend development team",
                    SprintLengthWeeks = 3,
                    Velocity = 18.0m,
                    MemberCount = 4,
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-15)
                }
            },
            TotalCount = 2
        };

        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ReturnsAsync(teamsResponse);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert
        Assert.Contains("Alpha Team", component.Markup);
        Assert.Contains("Beta Team", component.Markup);
        Assert.Contains("Frontend development team", component.Markup);
        Assert.Contains("Backend development team", component.Markup);
        Assert.Contains("5 members", component.Markup);
        Assert.Contains("4 members", component.Markup);
        Assert.Contains("25.5", component.Markup); // Velocity
        Assert.Contains("18.0", component.Markup); // Velocity
    }

    [Fact]
    public void TeamListComponent_WithActiveAndInactiveTeams_ShouldShowCorrectBadges()
    {
        // Arrange
        var teamsResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>
            {
                new()
                {
                    Id = 1,
                    Name = "Active Team",
                    Description = "Active team",
                    IsActive = true,
                    MemberCount = 3,
                    SprintLengthWeeks = 2,
                    Velocity = 20m,
                    CreatedDate = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Name = "Inactive Team",
                    Description = "Inactive team",
                    IsActive = false,
                    MemberCount = 2,
                    SprintLengthWeeks = 2,
                    Velocity = 15m,
                    CreatedDate = DateTime.UtcNow
                }
            },
            TotalCount = 2
        };

        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ReturnsAsync(teamsResponse);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert
        var markup = component.Markup;
        Assert.Contains("bg-success", markup); // Active badge
        Assert.Contains("bg-secondary", markup); // Inactive badge
        Assert.Contains("Active", markup);
        Assert.Contains("Inactive", markup);
    }

    [Fact]
    public void TeamListComponent_CreateTeamButton_ShouldBePresent()
    {
        // Arrange
        var emptyResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>(),
            TotalCount = 0
        };

        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ReturnsAsync(emptyResponse);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert
        var createButtons = component.FindAll("button:contains('Create')");
        Assert.NotEmpty(createButtons);
    }

    [Fact]
    public void TeamListComponent_WithTeams_ShouldShowViewAndEditButtons()
    {
        // Arrange
        var teamsResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Team",
                    Description = "Test description",
                    IsActive = true,
                    MemberCount = 3,
                    SprintLengthWeeks = 2,
                    Velocity = 20m,
                    CreatedDate = DateTime.UtcNow
                }
            },
            TotalCount = 1
        };

        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ReturnsAsync(teamsResponse);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert
        Assert.Contains("View", component.Markup);
        Assert.Contains("Edit", component.Markup);
        Assert.Contains("bi-eye", component.Markup);
        Assert.Contains("bi-pencil", component.Markup);
    }

    [Fact]
    public void TeamListComponent_WithServiceError_ShouldHandleGracefully()
    {
        // Arrange
        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act & Assert - Should not throw
        var component = RenderComponent<TeamListComponent>();
        
        // Component should still render, possibly with empty state
        Assert.NotNull(component.Markup);
    }

    [Fact]
    public void TeamListComponent_LoadingState_ShouldShowSpinner()
    {
        // Arrange
        var taskCompletionSource = new TaskCompletionSource<GetTeamsResponse>();
        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .Returns(taskCompletionSource.Task);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert - Should show loading spinner while awaiting
        Assert.Contains("spinner-border", component.Markup);
        Assert.Contains("Loading", component.Markup);
    }

    [Fact]
    public void TeamListComponent_SprintLengthAndVelocity_ShouldDisplayCorrectly()
    {
        // Arrange
        var teamsResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>
            {
                new()
                {
                    Id = 1,
                    Name = "Test Team",
                    Description = "Test team",
                    SprintLengthWeeks = 4,
                    Velocity = 32.7m,
                    MemberCount = 6,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            },
            TotalCount = 1
        };

        _teamServiceMock
            .Setup(x => x.GetTeamsAsync())
            .ReturnsAsync(teamsResponse);

        // Act
        var component = RenderComponent<TeamListComponent>();

        // Assert
        Assert.Contains("4 weeks", component.Markup);
        Assert.Contains("32.7", component.Markup);
        Assert.Contains("6 members", component.Markup);
    }
}