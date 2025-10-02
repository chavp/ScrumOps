using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ScrumOps.Api.Extensions;

/// <summary>
/// Extension methods for configuring observability features including logging, metrics, and tracing.
/// </summary>
public static class ObservabilityExtensions
{
    private const string ServiceName = "ScrumOps.Api";
    private const string ServiceVersion = "1.0.0";

    /// <summary>
    /// Configures comprehensive observability including structured logging, metrics, and distributed tracing.
    /// </summary>
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        // Configure Serilog with structured logging
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithProperty("ServiceName", ServiceName)
                .Enrich.WithProperty("ServiceVersion", ServiceVersion)
                .Filter.ByExcluding(logEvent => logEvent.MessageTemplate.Text.Contains("RequestPath like '/health%'"))
                .Filter.ByExcluding(logEvent => logEvent.MessageTemplate.Text.Contains("RequestPath like '/metrics%'"))
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    "logs/scrumops-.json",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 100_000_000, // 100MB
                    retainedFileCountLimit: 30,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.File(
                    "logs/scrumops-.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 100_000_000,
                    retainedFileCountLimit: 7);
        });

        // Add OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: ServiceName,
                    serviceVersion: ServiceVersion,
                    serviceInstanceId: Environment.MachineName))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("ScrumOps.Application")
                .AddMeter("ScrumOps.Infrastructure")
                .AddPrometheusExporter())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.SetTag("http.request.header.user-agent", request.Headers.UserAgent.ToString());
                        activity.SetTag("http.request.header.host", request.Headers.Host.ToString());
                    };
                    options.EnrichWithHttpResponse = (activity, response) =>
                    {
                        activity.SetTag("http.response.status_code", response.StatusCode);
                    };
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.SetDbStatementForStoredProcedure = true;
                    options.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.SetTag("db.operation", command.CommandType.ToString());
                    };
                })
                .AddSource("ScrumOps.Application")
                .AddSource("ScrumOps.Infrastructure")
                .SetSampler(new TraceIdRatioBasedSampler(GetSamplingRatio(builder.Environment)))
                .AddConsoleExporter());

        // Add custom activity sources
        builder.Services.AddSingleton(new ActivitySource("ScrumOps.Application"));
        builder.Services.AddSingleton(new ActivitySource("ScrumOps.Infrastructure"));

        // Add custom meters
        builder.Services.AddSingleton<Meter>(serviceProvider => 
            new Meter("ScrumOps.Application", ServiceVersion));

        return builder;
    }

    /// <summary>
    /// Configures the observability middleware pipeline.
    /// </summary>
    public static WebApplication UseObservability(this WebApplication app)
    {
        // Enable request logging with Serilog
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault());
                diagnosticContext.Set("TraceId", Activity.Current?.TraceId.ToString());
                diagnosticContext.Set("SpanId", Activity.Current?.SpanId.ToString());
                
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId", httpContext.User.Identity.Name);
                }
            };

            // Customize log level based on response status
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? Serilog.Events.LogEventLevel.Error
                : httpContext.Response.StatusCode > 499
                    ? Serilog.Events.LogEventLevel.Error
                    : httpContext.Response.StatusCode > 399
                        ? Serilog.Events.LogEventLevel.Warning
                        : Serilog.Events.LogEventLevel.Information;
        });

        // Add Prometheus metrics endpoint
        app.MapPrometheusScrapingEndpoint();

        return app;
    }

    /// <summary>
    /// Gets the sampling ratio based on the environment.
    /// </summary>
    private static double GetSamplingRatio(IWebHostEnvironment environment)
    {
        return environment.IsProduction() ? 0.1 : 1.0; // 10% in production, 100% in development
    }
}