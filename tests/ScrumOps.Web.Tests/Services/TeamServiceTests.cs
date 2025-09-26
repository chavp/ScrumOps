using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using ScrumOps.Shared.Contracts.Teams;
using ScrumOps.Web.Services;

namespace ScrumOps.Web.Tests.Services;

/// <summary>
/// Unit tests for TeamService HTTP client operations.
/// Tests error handling, HTTP communication, and service integration.
/// </summary>
public class TeamServiceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<TeamService>> _loggerMock;
    private readonly HttpClient _httpClient;
    private readonly TeamService _teamService;

    public TeamServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<TeamService>>();
        
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5225")
        };
        
        _teamService = new TeamService(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task GetTeamsAsync_WithSuccessfulResponse_ShouldReturnTeams()
    {
        // Arrange
        var expectedResponse = new GetTeamsResponse
        {
            Teams = new List<TeamSummary>
            {
                new() { Id = 1, Name = "Team Alpha", Description = "Test team", MemberCount = 5, IsActive = true }
            },
            TotalCount = 1
        };

        var jsonResponse = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString().EndsWith("/api/teams")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _teamService.GetTeamsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Teams);
        Assert.Equal("Team Alpha", result.Teams.First().Name);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetTeamsAsync_WithHttpError_ShouldReturnEmptyResponse()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _teamService.GetTeamsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Teams);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetTeamAsync_WithValidId_ShouldReturnTeamDetails()
    {
        // Arrange
        const int teamId = 1;
        var expectedTeam = new TeamDetailsResponse
        {
            Id = teamId,
            Name = "Team Alpha",
            Description = "Test team",
            SprintLengthWeeks = 2,
            Velocity = 25.5m,
            IsActive = true,
            Members = new List<TeamMember>
            {
                new() { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Developer" }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(expectedTeam);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString().EndsWith($"/api/teams/{teamId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _teamService.GetTeamAsync(teamId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teamId, result.Id);
        Assert.Equal("Team Alpha", result.Name);
        Assert.Single(result.Members);
        Assert.Equal("John Doe", result.Members.First().Name);
    }

    [Fact]
    public async Task GetTeamAsync_WithNotFound_ShouldThrowException()
    {
        // Arrange
        const int teamId = 999;
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _teamService.GetTeamAsync(teamId));
    }

    [Fact]
    public async Task CreateTeamAsync_WithValidRequest_ShouldReturnCreatedTeam()
    {
        // Arrange
        var createRequest = new CreateTeamRequest
        {
            Name = "New Team",
            Description = "Brand new team",
            SprintLengthWeeks = 3
        };

        var expectedResponse = new TeamDetailsResponse
        {
            Id = 2,
            Name = createRequest.Name,
            Description = createRequest.Description,
            SprintLengthWeeks = createRequest.SprintLengthWeeks,
            IsActive = true
        };

        var jsonResponse = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri!.ToString().EndsWith("/api/teams")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _teamService.CreateTeamAsync(createRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("New Team", result.Name);
        Assert.Equal("Brand new team", result.Description);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UpdateTeamAsync_WithValidRequest_ShouldReturnUpdatedTeam()
    {
        // Arrange
        const int teamId = 1;
        var updateRequest = new UpdateTeamRequest
        {
            Name = "Updated Team",
            Description = "Updated description",
            SprintLengthWeeks = 4
        };

        var expectedResponse = new TeamDetailsResponse
        {
            Id = teamId,
            Name = updateRequest.Name,
            Description = updateRequest.Description,
            SprintLengthWeeks = updateRequest.SprintLengthWeeks,
            IsActive = true
        };

        var jsonResponse = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri!.ToString().EndsWith($"/api/teams/{teamId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _teamService.UpdateTeamAsync(teamId, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teamId, result.Id);
        Assert.Equal("Updated Team", result.Name);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal(4, result.SprintLengthWeeks);
    }

    [Fact]
    public async Task DeleteTeamAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        const int teamId = 1;
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri!.ToString().EndsWith($"/api/teams/{teamId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _teamService.DeleteTeamAsync(teamId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTeamAsync_WithError_ShouldReturnFalse()
    {
        // Arrange
        const int teamId = 999;
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _teamService.DeleteTeamAsync(teamId);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}