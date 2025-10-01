using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Tests.Builders;

/// <summary>
/// Test builder for creating User entities with valid test data.
/// </summary>
public class UserBuilder
{
    private UserId _id = UserId.New();
    private TeamId _teamId = TeamId.New();
    private UserName _name = UserName.Create("Test User");
    private Email _email = Email.Create("test@example.com");
    private ScrumRole _role = ScrumRole.Developer;

    public UserBuilder WithId(UserId id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithTeamId(TeamId teamId)
    {
        _teamId = teamId;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _name = UserName.Create(name);
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = Email.Create(email);
        return this;
    }

    public UserBuilder WithRole(ScrumRole role)
    {
        _role = role;
        return this;
    }

    public UserBuilder AsProductOwner()
    {
        _role = ScrumRole.ProductOwner;
        return this;
    }

    public UserBuilder AsScrumMaster()
    {
        _role = ScrumRole.ScrumMaster;
        return this;
    }

    public UserBuilder AsDeveloper()
    {
        _role = ScrumRole.Developer;
        return this;
    }

    public User Build()
    {
        return new User(_id, _teamId, _name, _email, _role);
    }

    /// <summary>
    /// Creates a user with random but valid data.
    /// </summary>
    public static User Random(TeamId? teamId = null)
    {
        var random = new Random();
        var roles = new[] { ScrumRole.Developer, ScrumRole.ProductOwner, ScrumRole.ScrumMaster };
        
        return new UserBuilder()
            .WithTeamId(teamId ?? TeamId.New())
            .WithName($"User {random.Next(1000, 9999)}")
            .WithEmail($"user{random.Next(1000, 9999)}@example.com")
            .WithRole(roles[random.Next(roles.Length)])
            .Build();
    }

    /// <summary>
    /// Creates multiple users for a team with different roles.
    /// </summary>
    public static List<User> CreateTeamMembers(TeamId teamId, int developerCount = 3)
    {
        var users = new List<User>
        {
            new UserBuilder()
                .WithTeamId(teamId)
                .WithName("Product Owner")
                .WithEmail("po@example.com")
                .AsProductOwner()
                .Build(),
            
            new UserBuilder()
                .WithTeamId(teamId)
                .WithName("Scrum Master")
                .WithEmail("sm@example.com")
                .AsScrumMaster()
                .Build()
        };

        for (int i = 0; i < developerCount; i++)
        {
            users.Add(new UserBuilder()
                .WithTeamId(teamId)
                .WithName($"Developer {i + 1}")
                .WithEmail($"dev{i + 1}@example.com")
                .AsDeveloper()
                .Build());
        }

        return users;
    }
}