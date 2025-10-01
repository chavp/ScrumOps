using System;
using MediatR;
using ScrumOps.Application.Metrics.Services;

namespace ScrumOps.Application.Metrics.Commands;

/// <summary>
/// Command to export a report in different formats.
/// </summary>
public record ExportReportCommand(
    Guid ReportId, 
    ExportFormat Format) : IRequest<byte[]>;