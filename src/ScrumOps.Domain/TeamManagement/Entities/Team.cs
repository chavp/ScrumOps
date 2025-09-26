using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.Interfaces;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Events;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Domain.TeamManagement.Entities;

/// <summary>
/// Team aggregate root representing a Scrum team.
/// Manages team members, velocity, and team-related business rules.
/// </summary>
public class Team : Entity<TeamId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<User> _members = new();

    /// <summary>
    /// Gets the team name.
    /// </summary>
    public TeamName Name { get; private set; }

    /// <summary>
    /// Gets the team description.
    /// </summary>
    public TeamDescription Description { get; private set; }

    /// <summary>
    /// Gets the team's sprint length.
    /// </summary>
    public SprintLength SprintLength { get; private set; }

    /// <summary>
    /// Gets the team's current velocity.
    /// </summary>
    public Velocity CurrentVelocity { get; private set; }

    /// <summary>
    /// Gets the date when the team was created.
    /// </summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets whether the team is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the read-only list of team members.
    /// </summary>
    public IReadOnlyList<User> Members => _members.AsReadOnly();

    /// <summary>
    /// Gets the read-only list of domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private Team() : base()
    {
        Name = default!;
        Description = default!;
        SprintLength = default!;
        CurrentVelocity = default!;
    }

    /// <summary>
    /// Initializes a new instance of the Team class.
    /// </summary>
    /// <param name="id">The unique identifier for the team</param>
    /// <param name="name">The team name</param>
    /// <param name="description">The team description</param>
    /// <param name="sprintLength">The team's sprint length</param>
    public Team(TeamId id, TeamName name, TeamDescription description, SprintLength sprintLength)
        : base(id)
    {
        Name = name;
        Description = description;
        SprintLength = sprintLength;
        CurrentVelocity = Velocity.Zero;
        CreatedDate = DateTime.UtcNow;
        IsActive = true;

        _domainEvents.Add(new TeamCreatedEvent(Id, Name.Value, CreatedDate));
    }

    /// <summary>
    /// Adds a member to the team.
    /// </summary>
    /// <param name="user">The user to add to the team</param>
    /// <exception cref="DomainException">Thrown when business rules are violated</exception>
    public void AddMember(User user)
    {
        if (!IsActive)
        {
            throw new DomainException("Cannot add members to inactive team");
        }

        if (_members.Any(m => m.Email.Equals(user.Email)))
        {
            throw new DomainException("User with this email already exists in team");
        }

        if (HasUserWithRole(user.Role) && user.Role.IsSingletonRole())
        {
            throw new DomainException($"Team already has a {user.Role}");
        }

        _members.Add(user);
        _domainEvents.Add(new MemberAddedToTeamEvent(Id, user.Id, user.Role));
    }

    /// <summary>
    /// Removes a member from the team.
    /// </summary>
    /// <param name="userId">The ID of the user to remove</param>
    /// <exception cref="DomainException">Thrown when the user is not found</exception>
    public void RemoveMember(UserId userId)
    {
        var user = _members.FirstOrDefault(m => m.Id.Equals(userId));
        if (user == null)
        {
            throw new DomainException("User not found in team");
        }

        _members.Remove(user);
    }

    /// <summary>
    /// Updates the team's velocity based on completed sprints.
    /// </summary>
    /// <param name="newVelocity">The new velocity to set</param>
    public void UpdateVelocity(Velocity newVelocity)
    {
        var previousVelocity = CurrentVelocity;
        CurrentVelocity = newVelocity;

        _domainEvents.Add(new TeamVelocityUpdatedEvent(Id, previousVelocity, newVelocity));
    }

    /// <summary>
    /// Updates the team information.
    /// </summary>
    /// <param name="name">The new team name</param>
    /// <param name="description">The new team description</param>
    /// <param name="sprintLength">The new sprint length</param>
    public void UpdateTeamInfo(TeamName name, TeamDescription description, SprintLength sprintLength)
    {
        Name = name;
        Description = description;
        SprintLength = sprintLength;
    }

    /// <summary>
    /// Deactivates the team.
    /// </summary>
    /// <param name="reason">The reason for deactivation</param>
    public void Deactivate(string reason = "")
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        _domainEvents.Add(new TeamDeactivatedEvent(Id, DateTime.UtcNow, reason));
    }

    /// <summary>
    /// Reactivates the team.
    /// </summary>
    public void Reactivate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Clears all domain events from this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Checks if the team has a user with the specified role.
    /// </summary>
    /// <param name="role">The role to check for</param>
    /// <returns>True if a user with the role exists, false otherwise</returns>
    private bool HasUserWithRole(ScrumRole role)
    {
        return _members.Any(m => m.Role.Equals(role));
    }
}