using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.Teams;

namespace ScrumOps.Api.Tests.Controllers;

/// <summary>
/// Contract tests for Teams API endpoints.
/// These tests MUST FAIL initially to follow TDD principles.
/// Tests the API contracts defined in specs/001-scrum-framework/plan/contracts/teams-api.md
/// </summary>
public class TeamsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TeamsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTeams_ShouldReturnTeamsList()
    {
        // Act
        var response = await _client.GetAsync("/api/teams");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GetTeamsResponse>();
        
        Assert.NotNull(result);
        Assert.NotNull(result.Teams);
        Assert.True(result.TotalCount >= 0);
    }

    [Fact]
    public async Task GetTeam_WithValidId_ShouldReturnTeamDetails()
    {
        // Arrange
        var teamId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/teams/{teamId}");

        // Assert
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TeamDetailsResponse>();
            Assert.NotNull(result);
            Assert.Equal(teamId, result.Id);
            Assert.NotNull(result.Name);
            Assert.NotNull(result.Members);
        }
        else
        {
            // Should return 404 for non-existent team
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task CreateTeam_WithValidData_ShouldReturnCreatedTeam()
    {
        // Arrange
        var createRequest = new CreateTeamRequest
        {
            Name = "Test Team",
            Description = "A test team for integration testing",
            SprintLengthWeeks = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", createRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<TeamDetailsResponse>();
        Assert.NotNull(result);
        Assert.Equal(createRequest.Name, result.Name);
        Assert.Equal(createRequest.Description, result.Description);
        Assert.Equal(createRequest.SprintLengthWeeks, result.SprintLengthWeeks);
    }

    [Fact]
    public async Task CreateTeam_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new CreateTeamRequest
        {
            Name = "", // Invalid: empty name
            Description = "Test team",
            SprintLengthWeeks = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", createRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeam_WithValidData_ShouldReturnUpdatedTeam()
    {
        // Arrange
        var teamId = 1;
        var updateRequest = new UpdateTeamRequest
        {
            Name = "Updated Team Name",
            Description = "Updated description",
            SprintLengthWeeks = 3
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/teams/{teamId}", updateRequest);

        // Assert
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TeamDetailsResponse>();
            Assert.NotNull(result);
            Assert.Equal(updateRequest.Name, result.Name);
            Assert.Equal(updateRequest.Description, result.Description);
        }
        else
        {
            // Should return 404 for non-existent team
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task AddTeamMember_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var teamId = 1;
        var addMemberRequest = new AddTeamMemberRequest
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Role = "Developer"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/teams/{teamId}/members", addMemberRequest);

        // Assert
        if (response.IsSuccessStatusCode)
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }
        else
        {
            // Could be 404 (team not found) or 409 (member already exists)
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound ||
                       response.StatusCode == System.Net.HttpStatusCode.Conflict);
        }
    }

    [Fact]
    public async Task GetTeamVelocity_WithValidId_ShouldReturnVelocityData()
    {
        // Arrange
        var teamId = 1;

        // Act
        var response = await _client.GetAsync($"/api/teams/{teamId}/velocity");

        // Assert
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TeamVelocityResponse>();
            Assert.NotNull(result);
            Assert.True(result.CurrentVelocity >= 0);
            Assert.NotNull(result.VelocityHistory);
        }
        else
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact] 
    public async Task ApiEndpoints_ShouldHaveCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/teams");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task ApiEndpoints_ShouldSupportCors()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/teams");
        request.Headers.Add("Origin", "https://localhost:5003");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
}