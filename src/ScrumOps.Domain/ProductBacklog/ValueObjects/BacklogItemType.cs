namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Enumeration representing the type of a Product Backlog Item.
/// </summary>
public enum BacklogItemType
{
    /// <summary>
    /// A user story that describes functionality from the user's perspective.
    /// </summary>
    UserStory = 1,

    /// <summary>
    /// A technical task or infrastructure work.
    /// </summary>
    TechnicalTask = 2,

    /// <summary>
    /// A bug or defect that needs to be fixed.
    /// </summary>
    Bug = 3,

    /// <summary>
    /// A spike for research or investigation work.
    /// </summary>
    Spike = 4,

    /// <summary>
    /// An epic that groups multiple user stories.
    /// </summary>
    Epic = 5
}