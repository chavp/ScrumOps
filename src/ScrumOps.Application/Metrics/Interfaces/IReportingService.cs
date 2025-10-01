using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Application.Metrics.DTOs;
using ScrumOps.Domain.Metrics.Entities;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.Interfaces;

/// <summary>
/// Interface for reporting application service.
/// </summary>
public interface IReportingService
{
    /// <summary>
    /// Generates a new report for the specified team and period.
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="type">The report type</param>
    /// <param name="startDate">The start date of the reporting period</param>
    /// <param name="endDate">The end date of the reporting period</param>
    /// <param name="generatedBy">The user ID who is generating the report</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The generated report</returns>
    Task<Report> GenerateReportAsync(
        Guid teamId,
        ReportType type,
        DateTime startDate,
        DateTime endDate,
        Guid generatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reports by team for the specified period.
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of reports</returns>
    Task<IEnumerable<Report>> GetReportsByTeamAsync(
        Guid teamId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reports by type for the specified team and period.
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="type">The report type</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of reports</returns>
    Task<IEnumerable<Report>> GetReportsByTypeAsync(
        Guid teamId,
        ReportType type,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific report by ID.
    /// </summary>
    /// <param name="reportId">The report ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The report or null if not found</returns>
    Task<Report?> GetReportByIdAsync(
        Guid reportId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports a report to the specified format.
    /// </summary>
    /// <param name="reportId">The report ID</param>
    /// <param name="format">The export format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The exported report data</returns>
    Task<byte[]> ExportReportAsync(
        Guid reportId,
        ReportExportFormat format,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Supported report export formats.
/// </summary>
public enum ReportExportFormat
{
    Pdf,
    Excel,
    Csv,
    Json
}