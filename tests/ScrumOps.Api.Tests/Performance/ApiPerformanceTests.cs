using Microsoft.AspNetCore.Mvc.Testing;
using ScrumOps.Shared.Contracts.Teams;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net;

namespace ScrumOps.Api.Tests.Performance;

/// <summary>
/// Performance tests to ensure API endpoints meet the <200ms requirement.
/// Tests response times under various load conditions.
/// </summary>
public class ApiPerformanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private const int PerformanceThresholdMs = 200;

    public ApiPerformanceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTeams_ShouldRespondWithin200ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/api/teams");
        stopwatch.Stop();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(stopwatch.ElapsedMilliseconds < PerformanceThresholdMs,
            $"GET /api/teams took {stopwatch.ElapsedMilliseconds}ms, expected < {PerformanceThresholdMs}ms");
    }

    [Fact]
    public async Task GetTeamById_ShouldRespondWithin200ms()
    {
        // Arrange
        const int teamId = 1; // Using existing team ID
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync($"/api/teams/{teamId}");
        stopwatch.Stop();

        // Assert
        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound);
        Assert.True(stopwatch.ElapsedMilliseconds < PerformanceThresholdMs,
            $"GET /api/teams/{teamId} took {stopwatch.ElapsedMilliseconds}ms, expected < {PerformanceThresholdMs}ms");
    }

    [Fact]
    public async Task HealthCheck_ShouldRespondWithin50ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/health");
        stopwatch.Stop();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(stopwatch.ElapsedMilliseconds < 50,
            $"GET /health took {stopwatch.ElapsedMilliseconds}ms, expected < 50ms");
    }

    [Fact]
    public async Task CreateTeam_ShouldRespondWithin200ms()
    {
        // Arrange
        var createRequest = new CreateTeamRequest
        {
            Name = "Performance Test Team",
            Description = "Team created for performance testing",
            SprintLengthWeeks = 2
        };
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", createRequest);
        stopwatch.Stop();

        // Assert
        Assert.True(response.StatusCode is HttpStatusCode.Created or 
                   HttpStatusCode.BadRequest or 
                   HttpStatusCode.OK);
        Assert.True(stopwatch.ElapsedMilliseconds < PerformanceThresholdMs,
            $"POST /api/teams took {stopwatch.ElapsedMilliseconds}ms, expected < {PerformanceThresholdMs}ms");
    }

    [Fact]
    public async Task ConcurrentRequests_ShouldMaintainPerformance()
    {
        // Arrange
        const int concurrentRequests = 10;
        var tasks = new List<Task<(HttpResponseMessage Response, long ElapsedMs)>>();

        // Act
        for (int i = 0; i < concurrentRequests; i++)
        {
            tasks.Add(MeasureRequestTime(() => _client.GetAsync("/api/teams")));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        foreach (var (response, elapsedMs) in results)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(elapsedMs < PerformanceThresholdMs * 2, // Allow 2x threshold for concurrent load
                $"Concurrent request took {elapsedMs}ms, expected < {PerformanceThresholdMs * 2}ms");
        }

        // Verify average performance
        var averageTime = results.Average(r => r.ElapsedMs);
        Assert.True(averageTime < PerformanceThresholdMs,
            $"Average response time was {averageTime}ms, expected < {PerformanceThresholdMs}ms");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task MultipleSequentialRequests_ShouldMaintainConsistentPerformance(int requestCount)
    {
        // Arrange
        var responseTimes = new List<long>();

        // Act
        for (int i = 0; i < requestCount; i++)
        {
            var (_, elapsedMs) = await MeasureRequestTime(() => _client.GetAsync("/api/teams"));
            responseTimes.Add(elapsedMs);
        }

        // Assert
        var averageTime = responseTimes.Average();
        var maxTime = responseTimes.Max();

        Assert.True(averageTime < PerformanceThresholdMs,
            $"Average response time over {requestCount} requests was {averageTime}ms");
        
        Assert.True(maxTime < PerformanceThresholdMs * 2,
            $"Max response time over {requestCount} requests was {maxTime}ms");
    }

    private static async Task<(HttpResponseMessage Response, long ElapsedMs)> MeasureRequestTime(
        Func<Task<HttpResponseMessage>> requestFunc)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await requestFunc();
        stopwatch.Stop();
        return (response, stopwatch.ElapsedMilliseconds);
    }
}