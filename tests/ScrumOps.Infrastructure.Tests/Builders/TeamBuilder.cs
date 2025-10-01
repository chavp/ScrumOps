using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Tests.Builders;

/// <summary>
/// Test builder for creating Team entities with valid test data.
/// </summary>
public class TeamBuilder
{
    private TeamId _id = TeamId.New();
    private TeamName _name = TeamName.Create("Test Team");
    private TeamDescription _description = TeamDescription.Create("A test team for unit testing");
    private SprintLength _sprintLength = SprintLength.Create(2);

    public TeamBuilder WithId(TeamId id)
    {
        _id = id;
        return this;
    }

    public TeamBuilder WithName(string name)
    {
        _name = TeamName.Create(name);
        return this;
    }

    public TeamBuilder WithDescription(string description)
    {
        _description = TeamDescription.Create(description);
        return this;
    }

    public TeamBuilder WithSprintLength(int weeks)
    {
        _sprintLength = SprintLength.Create(weeks);
        return this;
    }

    public Team Build()
    {
        return new Team(_id, _name, _description, _sprintLength);
    }

    /// <summary>
    /// Creates a team with random but valid data.
    /// </summary>
    public static Team Random()
    {
        var random = new Random();
        return new TeamBuilder()
            .WithName($"Team {random.Next(1000, 9999)}")
            .WithDescription($"Test team created at {DateTime.UtcNow}")
            .WithSprintLength(random.Next(1, 5))
            .Build();
    }

    /// <summary>
    /// Creates multiple teams with unique names.
    /// </summary>
    public static List<Team> CreateMultiple(int count)
    {
        var teams = new List<Team>();
        for (int i = 0; i < count; i++)
        {
            teams.Add(new TeamBuilder()
                .WithName($"Team {i + 1}")
                .WithDescription($"Test team {i + 1}")
                .WithSprintLength((i % 4) + 1)
                .Build());
        }
        return teams;
    }
}