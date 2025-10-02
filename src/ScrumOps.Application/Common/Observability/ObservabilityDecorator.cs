using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ScrumOps.Application.Common.Observability;

/// <summary>
/// Base decorator for adding observability to application services.
/// </summary>
public abstract class ObservabilityDecorator<TService>
{
    protected readonly TService _decoratedService;
    protected readonly ILogger<TService> _logger;
    protected readonly ActivitySource _activitySource;

    protected ObservabilityDecorator(TService decoratedService, ILogger<TService> logger)
    {
        _decoratedService = decoratedService;
        _logger = logger;
        _activitySource = new ActivitySource("ScrumOps.Application");
    }

    /// <summary>
    /// Executes an operation with observability features.
    /// </summary>
    protected async Task<TResult> ExecuteWithObservabilityAsync<TResult>(
        string operationName,
        Func<Task<TResult>> operation,
        object? request = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity(operationName);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Add request details to activity if provided
            if (request != null)
            {
                activity?.SetTag("request.type", request.GetType().Name);
                activity?.SetTag("request.data", System.Text.Json.JsonSerializer.Serialize(request));
            }

            _logger.LogInformation("Starting operation: {OperationName}", operationName);

            var result = await operation();

            stopwatch.Stop();
            
            // Add result details to activity
            activity?.SetTag("result.type", typeof(TResult).Name);
            activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetTag("operation.success", true);

            _logger.LogInformation(
                "Operation {OperationName} completed successfully in {ElapsedMs}ms",
                operationName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Record exception in activity
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetTag("operation.success", false);

            _logger.LogError(ex,
                "Operation {OperationName} failed after {ElapsedMs}ms: {ErrorMessage}",
                operationName, stopwatch.ElapsedMilliseconds, ex.Message);

            throw;
        }
    }

    /// <summary>
    /// Executes an operation with observability features (void return).
    /// </summary>
    protected async Task ExecuteWithObservabilityAsync(
        string operationName,
        Func<Task> operation,
        object? request = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity(operationName);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Add request details to activity if provided
            if (request != null)
            {
                activity?.SetTag("request.type", request.GetType().Name);
                activity?.SetTag("request.data", System.Text.Json.JsonSerializer.Serialize(request));
            }

            _logger.LogInformation("Starting operation: {OperationName}", operationName);

            await operation();

            stopwatch.Stop();
            
            // Add operation details to activity
            activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetTag("operation.success", true);

            _logger.LogInformation(
                "Operation {OperationName} completed successfully in {ElapsedMs}ms",
                operationName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Record exception in activity
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetTag("operation.success", false);

            _logger.LogError(ex,
                "Operation {OperationName} failed after {ElapsedMs}ms: {ErrorMessage}",
                operationName, stopwatch.ElapsedMilliseconds, ex.Message);

            throw;
        }
    }
}