using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Application.Metrics.DTOs;
using ScrumOps.Domain.Metrics.Entities;

namespace ScrumOps.Application.Metrics.Queries;

/// <summary>
/// Query to get a specific report by ID.
/// </summary>
public record GetReportQuery(Guid ReportId) : IRequest<ReportDto?>;

/// <summary>
/// Query to get all reports for a team.
/// </summary>
public record GetTeamReportsQuery(
    Guid TeamId, 
    int Skip = 0, 
    int Take = 20) : IRequest<IEnumerable<ReportDto>>;

/// <summary>
/// Query to get reports by type and period.
/// </summary>
public record GetReportsByTypeQuery(
    Guid TeamId, 
    ReportType ReportType, 
    DateTime? StartDate = null, 
    DateTime? EndDate = null) : IRequest<IEnumerable<ReportDto>>;