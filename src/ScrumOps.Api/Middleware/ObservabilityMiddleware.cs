using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ScrumOps.Api.Middleware;

/// <summary>
/// Middleware for collecting request-level observability data including custom metrics and tracing.
/// </summary>
public class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ObservabilityMiddleware> _logger;
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Counter<long> _errorCounter;
    private readonly ActivitySource _activitySource;

    public ObservabilityMiddleware(
        RequestDelegate next,
        ILogger<ObservabilityMiddleware> logger,
        Meter meter)
    {
        _next = next;
        _logger = logger;
        _meter = meter;
        _activitySource = new ActivitySource("ScrumOps.Api");

        // Initialize metrics
        _requestCounter = _meter.CreateCounter<long>(
            "http_requests_total",
            description: "Total number of HTTP requests");

        _requestDuration = _meter.CreateHistogram<double>(
            "http_request_duration_seconds",
            unit: "s",
            description: "Duration of HTTP requests");

        _errorCounter = _meter.CreateCounter<long>(
            "http_errors_total",
            description: "Total number of HTTP errors");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var path = context.Request.Path.Value ?? "";
        var method = context.Request.Method;

        // Skip observability endpoints to avoid noise
        if (path.StartsWith("/metrics") || path.StartsWith("/health"))
        {
            await _next(context);
            return;
        }

        using var activity = _activitySource.StartActivity($"{method} {path}");
        
        // Add request context to activity
        activity?.SetTag("http.method", method);
        activity?.SetTag("http.route", path);
        activity?.SetTag("http.scheme", context.Request.Scheme);
        activity?.SetTag("http.host", context.Request.Host.ToString());
        activity?.SetTag("user_agent", context.Request.Headers.UserAgent.ToString());

        // Add user information if available
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            activity?.SetTag("user.id", context.User.Identity.Name);
        }

        // Add correlation ID to response headers
        var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        context.Response.Headers.Append("X-Correlation-ID", correlationId);

        var statusCode = 0;
        Exception? exception = null;

        try
        {
            await _next(context);
            statusCode = context.Response.StatusCode;
        }
        catch (Exception ex)
        {
            exception = ex;
            statusCode = 500;
            
            // Add exception details to activity
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalSeconds;

            // Record metrics
            var tags = new[]
            {
                new KeyValuePair<string, object?>("method", method),
                new KeyValuePair<string, object?>("route", GetNormalizedRoute(path)),
                new KeyValuePair<string, object?>("status_code", statusCode)
            };

            _requestCounter.Add(1, tags);
            _requestDuration.Record(duration, tags);

            // Record error if status code indicates an error
            if (statusCode >= 400)
            {
                var errorTags = new[]
                {
                    new KeyValuePair<string, object?>("method", method),
                    new KeyValuePair<string, object?>("route", GetNormalizedRoute(path)),
                    new KeyValuePair<string, object?>("status_code", statusCode),
                    new KeyValuePair<string, object?>("error_type", GetErrorType(statusCode))
                };

                _errorCounter.Add(1, errorTags);
            }

            // Update activity with response information
            activity?.SetTag("http.status_code", statusCode);
            activity?.SetTag("http.response.size", context.Response.ContentLength ?? 0);

            // Log request completion
            _logger.LogInformation(
                "HTTP {Method} {Path} completed in {Duration}ms with status {StatusCode}",
                method, path, stopwatch.ElapsedMilliseconds, statusCode);
        }
    }

    /// <summary>
    /// Normalizes route paths to avoid high cardinality in metrics.
    /// </summary>
    private static string GetNormalizedRoute(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "/";

        // Replace IDs with placeholders to reduce cardinality
        var segments = path.Split('/');
        for (int i = 0; i < segments.Length; i++)
        {
            if (Guid.TryParse(segments[i], out _) || 
                (int.TryParse(segments[i], out _) && segments[i].Length > 0))
            {
                segments[i] = "{id}";
            }
        }

        return string.Join("/", segments);
    }

    /// <summary>
    /// Gets error type based on status code.
    /// </summary>
    private static string GetErrorType(int statusCode)
    {
        return statusCode switch
        {
            >= 400 and < 500 => "client_error",
            >= 500 => "server_error",
            _ => "unknown"
        };
    }
}