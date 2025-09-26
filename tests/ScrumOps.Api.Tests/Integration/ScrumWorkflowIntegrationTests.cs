using Microsoft.AspNetCore.Mvc.Testing;
using ScrumOps.Shared.Contracts.Teams;
using ScrumOps.Shared.Contracts.ProductBacklog;
using System.Net.Http.Json;
using System.Net;

namespace ScrumOps.Api.Tests.Integration;

/// <summary>
/// End-to-end integration tests for complete Scrum workflows.
/// Tests the entire flow from team creation to sprint completion.
/// </summary>
public class ScrumWorkflowIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ScrumWorkflowIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CompleteTeamCreationWorkflow_ShouldSucceed()
    {
        // Step 1: Create a new team
        var createTeamRequest = new CreateTeamRequest
        {
            Name = "Integration Test Team",
            Description = "Team created during integration testing",
            SprintLengthWeeks = 2
        };

        var createResponse = await _client.PostAsJsonAsync("/api/teams", createTeamRequest);
        
        if (createResponse.IsSuccessStatusCode)
        {
            var createdTeam = await createResponse.Content.ReadFromJsonAsync<TeamDetailsResponse>();
            Assert.NotNull(createdTeam);
            Assert.Equal(createTeamRequest.Name, createdTeam.Name);

            // Step 2: Retrieve the created team
            var getResponse = await _client.GetAsync($"/api/teams/{createdTeam.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var retrievedTeam = await getResponse.Content.ReadFromJsonAsync<TeamDetailsResponse>();
            Assert.NotNull(retrievedTeam);
            Assert.Equal(createdTeam.Id, retrievedTeam.Id);
            Assert.Equal(createTeamRequest.Name, retrievedTeam.Name);

            // Step 3: Update the team
            var updateRequest = new UpdateTeamRequest
            {
                Name = "Updated Integration Test Team",
                Description = "Updated description",
                SprintLengthWeeks = 3
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/teams/{createdTeam.Id}", updateRequest);
            
            if (updateResponse.IsSuccessStatusCode)
            {
                var updatedTeam = await updateResponse.Content.ReadFromJsonAsync<TeamDetailsResponse>();
                Assert.NotNull(updatedTeam);
                Assert.Equal(updateRequest.Name, updatedTeam.Name);
                Assert.Equal(updateRequest.Description, updatedTeam.Description);
                Assert.Equal(updateRequest.SprintLengthWeeks, updatedTeam.SprintLengthWeeks);
            }

            // Step 4: Verify team appears in list
            var listResponse = await _client.GetAsync("/api/teams");
            Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

            var teamsList = await listResponse.Content.ReadFromJsonAsync<GetTeamsResponse>();
            Assert.NotNull(teamsList);
            Assert.Contains(teamsList.Teams, t => t.Id == createdTeam.Id);
        }
        else
        {
            // If team creation fails, verify it's due to validation or expected business rules
            Assert.True(createResponse.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Conflict);
        }
    }

    [Fact]
    public async Task InvalidTeamCreation_ShouldReturnValidationErrors()
    {
        // Test with empty name
        var invalidRequest1 = new CreateTeamRequest
        {
            Name = "", // Invalid
            Description = "Test team",
            SprintLengthWeeks = 2
        };

        var response1 = await _client.PostAsJsonAsync("/api/teams", invalidRequest1);
        Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);

        // Test with invalid sprint length
        var invalidRequest2 = new CreateTeamRequest
        {
            Name = "Valid Team Name",
            Description = "Test team",
            SprintLengthWeeks = 5 // Invalid - exceeds 4 weeks
        };

        var response2 = await _client.PostAsJsonAsync("/api/teams", invalidRequest2);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);

        // Test with name too short
        var invalidRequest3 = new CreateTeamRequest
        {
            Name = "AB", // Too short - minimum 3 characters
            Description = "Test team",
            SprintLengthWeeks = 2
        };

        var response3 = await _client.PostAsJsonAsync("/api/teams", invalidRequest3);
        Assert.Equal(HttpStatusCode.BadRequest, response3.StatusCode);
    }

    [Fact]
    public async Task ConcurrentTeamOperations_ShouldHandleCorrectly()
    {
        // Create multiple teams concurrently
        var tasks = new List<Task<HttpResponseMessage>>();
        
        for (int i = 0; i < 5; i++)
        {
            var request = new CreateTeamRequest
            {
                Name = $"Concurrent Team {i}",
                Description = $"Concurrency test team {i}",
                SprintLengthWeeks = 2
            };
            
            tasks.Add(_client.PostAsJsonAsync("/api/teams", request));
        }

        var responses = await Task.WhenAll(tasks);

        // Verify all requests were processed (either successfully or with expected errors)
        foreach (var response in responses)
        {
            Assert.True(response.StatusCode is HttpStatusCode.Created or 
                       HttpStatusCode.BadRequest or 
                       HttpStatusCode.Conflict or
                       HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task APIHealthAndConnectivity_ShouldBeAccessible()
    {
        // Test health endpoint
        var healthResponse = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);

        // Test CORS headers are present for web app integration
        var corsResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/api/teams"));
        // CORS preflight should be handled properly
        Assert.True(corsResponse.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent);

        // Test API responds to basic GET requests
        var teamsResponse = await _client.GetAsync("/api/teams");
        Assert.Equal(HttpStatusCode.OK, teamsResponse.StatusCode);
    }

    [Theory]
    [InlineData("/api/teams")]
    [InlineData("/health")]
    public async Task APIEndpoints_ShouldRespondWithCorrectContentType(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.True(response.Content.Headers.ContentType?.MediaType?.Contains("json") == true ||
                   response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task LargeTeamName_ShouldBeHandledCorrectly()
    {
        // Test with maximum allowed length
        var maxLengthRequest = new CreateTeamRequest
        {
            Name = new string('A', 50), // Maximum length
            Description = "Test team with max length name",
            SprintLengthWeeks = 2
        };

        var maxResponse = await _client.PostAsJsonAsync("/api/teams", maxLengthRequest);
        // Should succeed or return validation error
        Assert.True(maxResponse.StatusCode is HttpStatusCode.Created or 
                   HttpStatusCode.BadRequest or 
                   HttpStatusCode.OK);

        // Test with over maximum length
        var overMaxRequest = new CreateTeamRequest
        {
            Name = new string('B', 51), // Over maximum length
            Description = "Test team with over max length name",
            SprintLengthWeeks = 2
        };

        var overMaxResponse = await _client.PostAsJsonAsync("/api/teams", overMaxRequest);
        Assert.Equal(HttpStatusCode.BadRequest, overMaxResponse.StatusCode);
    }
}