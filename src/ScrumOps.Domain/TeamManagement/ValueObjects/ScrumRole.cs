using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.TeamManagement.ValueObjects;

/// <summary>
/// Value object representing a Scrum role within a team.
/// Defines the standard Scrum roles and their characteristics.
/// </summary>
public class ScrumRole : ValueObject
{
    /// <summary>
    /// Product Owner role - responsible for the product backlog and stakeholder communication.
    /// </summary>
    public static readonly ScrumRole ProductOwner = new("Product Owner", true);

    /// <summary>
    /// Scrum Master role - facilitates the Scrum process and removes impediments.
    /// </summary>
    public static readonly ScrumRole ScrumMaster = new("Scrum Master", true);

    /// <summary>
    /// Developer role - responsible for creating the product increment.
    /// </summary>
    public static readonly ScrumRole Developer = new("Developer", false);

    /// <summary>
    /// Gets the name of the role.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets whether this role is a singleton (only one person can have this role per team).
    /// </summary>
    public bool IsSingleton { get; }

    /// <summary>
    /// Private constructor to enforce predefined roles.
    /// </summary>
    /// <param name="name">The role name</param>
    /// <param name="isSingleton">Whether this role is singleton</param>
    private ScrumRole(string name, bool isSingleton)
    {
        Name = name;
        IsSingleton = isSingleton;
    }

    /// <summary>
    /// Determines whether this role is a singleton role (only one per team).
    /// </summary>
    /// <returns>True if this is a singleton role, false otherwise</returns>
    public bool IsSingletonRole() => IsSingleton;

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The role name and singleton flag</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Name;
        yield return IsSingleton;
    }

    /// <summary>
    /// Returns the string representation of the role.
    /// </summary>
    /// <returns>The role name</returns>
    public override string ToString() => Name;

    /// <summary>
    /// Creates a ScrumRole from a string representation.
    /// </summary>
    /// <param name="roleName">The role name string</param>
    /// <returns>The corresponding ScrumRole</returns>
    /// <exception cref="ArgumentException">Thrown when the role name is not recognized</exception>
    public static ScrumRole FromString(string roleName)
    {
        return roleName?.ToLowerInvariant() switch
        {
            "product owner" or "productowner" => ProductOwner,
            "scrum master" or "scrummaster" => ScrumMaster,
            "developer" => Developer,
            _ => throw new ArgumentException($"Unknown role: {roleName}", nameof(roleName))
        };
    }
}