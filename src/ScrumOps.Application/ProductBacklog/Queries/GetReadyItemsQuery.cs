using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Queries;

/// <summary>
/// Query to get items ready for sprint planning.
/// </summary>
public record GetReadyItemsQuery(TeamId TeamId) : IRequest<ReadyItemsResponse?>;

/// <summary>
/// Response containing items ready for sprint planning.
/// </summary>
public class ReadyItemsResponse
{
    public List<ReadyItemDto> ReadyItems { get; set; } = new();
    public int TotalReadyPoints { get; set; }
    public List<int> RecommendedForNextSprint { get; set; } = new();
}

/// <summary>
/// DTO for ready backlog item.
/// </summary>
public class ReadyItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int StoryPoints { get; set; }
    public int Priority { get; set; }
    public bool HasAcceptanceCriteria { get; set; }
    public bool IsEstimated { get; set; }
    public List<string> Dependencies { get; set; } = new();
}