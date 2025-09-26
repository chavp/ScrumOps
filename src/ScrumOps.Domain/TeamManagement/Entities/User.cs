using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Domain.TeamManagement.Entities;

/// <summary>
/// User entity representing a team member.
/// Contains user information and role within the team.
/// </summary>
public class User : Entity<UserId>
{
    /// <summary>
    /// Gets the ID of the team this user belongs to.
    /// </summary>
    public TeamId TeamId { get; private set; }

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public UserName Name { get; private set; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the user's role within the team.
    /// </summary>
    public ScrumRole Role { get; private set; }

    /// <summary>
    /// Gets the date when the user was created.
    /// </summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets the date when the user last logged in.
    /// </summary>
    public DateTime? LastLoginDate { get; private set; }

    /// <summary>
    /// Gets whether the user is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private User() : base()
    {
        TeamId = default!;
        Name = default!;
        Email = default!;
        Role = default!;
    }

    /// <summary>
    /// Initializes a new instance of the User class.
    /// </summary>
    /// <param name="id">The unique identifier for the user</param>
    /// <param name="teamId">The ID of the team this user belongs to</param>
    /// <param name="name">The user's full name</param>
    /// <param name="email">The user's email address</param>
    /// <param name="role">The user's role within the team</param>
    public User(UserId id, TeamId teamId, UserName name, Email email, ScrumRole role)
        : base(id)
    {
        TeamId = teamId;
        Name = name;
        Email = email;
        Role = role;
        CreatedDate = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// Updates the user's last login date.
    /// </summary>
    /// <param name="loginTime">The date and time of the login</param>
    public void UpdateLastLogin(DateTime loginTime)
    {
        LastLoginDate = loginTime;
    }

    /// <summary>
    /// Changes the user's role within the team.
    /// </summary>
    /// <param name="newRole">The new role to assign to the user</param>
    public void ChangeRole(ScrumRole newRole)
    {
        if (Role.Equals(newRole))
        {
            return;
        }

        Role = newRole;
    }

    /// <summary>
    /// Deactivates the user.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Reactivates the user.
    /// </summary>
    public void Reactivate()
    {
        IsActive = true;
    }
}