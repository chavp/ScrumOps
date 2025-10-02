# ScrumOps Observability Guide

This document provides comprehensive information about the observability features implemented in the ScrumOps application, including logging, metrics, and distributed tracing.

## Overview

The ScrumOps application implements comprehensive observability using OpenTelemetry standards and industry best practices. The observability stack includes:

- **Structured Logging** with Serilog and JSON formatting
- **Metrics Collection** with OpenTelemetry and Prometheus
- **Distributed Tracing** with OpenTelemetry and Jaeger
- **Monitoring Dashboards** with Grafana

## Architecture

```
┌─────────────────┐
│   ScrumOps API  │──┐
│   (Port 8080)   │  │
└─────────────────┘  │
                     │
┌─────────────────┐  │  ┌─────────────────┐
│   Prometheus    │◄─┼──┤   Grafana       │
│   (Port 9090)   │  │  │   (Port 3000)   │
└─────────────────┘  │  └─────────────────┘
                     │
┌─────────────────┐  │
│     Jaeger      │◄─┘
│   (Port 16686)  │
└─────────────────┘
```

## Features

### 1. Structured Logging

#### Configuration
- **Format**: JSON (CompactJsonFormatter) for machine readability
- **Levels**: DEBUG, INFO, WARN, ERROR, FATAL
- **Rotation**: Daily rotation with size limits (100MB per file)
- **Retention**: 30 days for JSON logs, 7 days for text logs
- **Sensitive Data**: Automatic masking of sensitive information

#### Log Context
Each log entry includes:
- Timestamp (ISO 8601)
- Service name and version
- Machine name and process information
- Trace ID and Span ID for correlation
- Request ID
- User ID (when authenticated)

#### Example Log Entry
```json
{
  "@t": "2024-01-15T10:30:00.123Z",
  "@l": "Information",
  "@m": "HTTP GET /api/teams completed in 45ms with status 200",
  "ServiceName": "ScrumOps.Api",
  "ServiceVersion": "1.0.0",
  "TraceId": "4bf92f3577b34da6a3ce929d0e0e4736",
  "SpanId": "00f067aa0ba902b7",
  "RequestId": "0HN1GVGVG1N01:00000001",
  "MachineName": "scrumops-api",
  "ProcessId": 1234
}
```

### 2. Metrics Collection

#### Application Metrics
- **Request Rate**: `http_requests_total` - Total HTTP requests
- **Response Time**: `http_request_duration_seconds` - Request latency percentiles
- **Error Rate**: `http_errors_total` - HTTP errors by status code
- **Success Rate**: Calculated from request/error metrics

#### Business Metrics
- **Teams**: `scrumops_teams_created_total`, `scrumops_active_teams_current`
- **Sprints**: `scrumops_sprints_started_total`, `scrumops_sprints_completed_total`, `scrumops_active_sprints_current`
- **Backlog Items**: `scrumops_backlog_items_created_total`, `scrumops_backlog_items_completed_total`
- **Performance**: `scrumops_sprint_duration_days`, `scrumops_sprint_completion_rate`

#### System Metrics
- **Runtime**: CPU usage, memory usage, GC information
- **Process**: Thread count, handle count
- **Network**: Request/response sizes

#### Prometheus Endpoint
Metrics are exposed at: `http://localhost:8080/metrics`

### 3. Distributed Tracing

#### Features
- **Automatic Instrumentation**: HTTP requests, database calls, external APIs
- **Custom Spans**: Business logic operations
- **Context Propagation**: W3C Trace Context standard
- **Sampling**: Configurable rate-based sampling (100% dev, 10% prod)

#### Trace Information
Each span includes:
- Operation name and duration
- HTTP method, URL, status code
- Database queries and parameters
- Exception details (when errors occur)
- Custom business attributes

#### Jaeger UI
Access traces at: `http://localhost:16686`

## Configuration

### Environment-Specific Settings

#### Development (`appsettings.Development.json`)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  },
  "OpenTelemetry": {
    "Sampling": { "Ratio": 1.0 }
  },
  "Observability": {
    "SensitiveDataMasking": false
  }
}
```

#### Production (`appsettings.Production.json`)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  },
  "OpenTelemetry": {
    "Sampling": { "Ratio": 0.1 }
  },
  "Observability": {
    "SensitiveDataMasking": true
  }
}
```

## Deployment

### Docker Compose
The complete observability stack is deployed using Docker Compose:

```bash
# Start all services including observability stack
docker-compose up -d

# View logs
docker-compose logs -f api

# Scale services
docker-compose up -d --scale api=3
```

### Services Ports
- **ScrumOps API**: http://localhost:8080
- **Grafana**: http://localhost:3000 (admin/admin123)
- **Prometheus**: http://localhost:9090
- **Jaeger**: http://localhost:16686
- **pgAdmin**: http://localhost:8081

## Monitoring and Alerting

### Grafana Dashboards

#### ScrumOps Overview Dashboard
- HTTP request rate and latency
- Active teams and sprints
- Error rates and success rates
- Business KPIs

#### Key Metrics to Monitor
1. **Request Latency**: 95th percentile < 2s
2. **Error Rate**: < 1% for 4xx/5xx errors
3. **Availability**: > 99.9% uptime
4. **Business Metrics**: Sprint completion rates, team velocity

### Health Checks

#### Endpoints
- **Health**: `/health` - General application health
- **Readiness**: `/health/ready` - Database connectivity
- **Liveness**: `/health/live` - Application responsiveness

## Usage Examples

### Adding Custom Metrics

```csharp
public class CustomService
{
    private readonly BusinessMetricsService _metrics;

    public async Task CreateTeamAsync(CreateTeamRequest request)
    {
        // Business logic...
        
        // Record business metric
        _metrics.RecordTeamCreated(team.Name);
    }
}
```

### Adding Custom Tracing

```csharp
public class TeamService : ObservabilityDecorator<ITeamService>
{
    public async Task<Team> GetTeamAsync(TeamId id)
    {
        return await ExecuteWithObservabilityAsync(
            "GetTeam",
            () => _decoratedService.GetTeamAsync(id),
            new { TeamId = id.Value }
        );
    }
}
```

### Structured Logging

```csharp
public class TeamController : ControllerBase
{
    private readonly ILogger<TeamController> _logger;

    public async Task<IActionResult> CreateTeam(CreateTeamRequest request)
    {
        _logger.LogInformation("Creating team {TeamName} with {MemberCount} members", 
            request.Name, request.Members.Count);
        
        try
        {
            var result = await _teamService.CreateTeamAsync(request);
            
            _logger.LogInformation("Team {TeamId} created successfully", result.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create team {TeamName}", request.Name);
            throw;
        }
    }
}
```

## Best Practices

### Logging
1. Use structured logging with meaningful context
2. Avoid logging sensitive information
3. Use appropriate log levels
4. Include correlation IDs for request tracing

### Metrics
1. Keep cardinality low (< 10,000 unique combinations)
2. Use consistent naming conventions
3. Include relevant labels but avoid high-cardinality values
4. Monitor both technical and business metrics

### Tracing
1. Create spans for important operations
2. Add relevant attributes and tags
3. Use sampling to control overhead
4. Propagate context across service boundaries

### Performance
1. Observability overhead should be < 5%
2. Use asynchronous logging
3. Configure appropriate buffer sizes
4. Monitor observability system health

## Troubleshooting

### Common Issues

#### High Cardinality Metrics
**Problem**: Too many unique metric combinations
**Solution**: Review labels and remove high-cardinality dimensions

#### Missing Traces
**Problem**: Traces not appearing in Jaeger
**Solution**: Check sampling configuration and endpoint connectivity

#### Log Volume
**Problem**: Excessive log volume
**Solution**: Adjust log levels and filtering rules

#### Dashboard Loading Issues
**Problem**: Grafana dashboards not loading
**Solution**: Verify Prometheus data source connectivity

### Debugging Commands

```bash
# Check API health
curl http://localhost:8080/health

# View metrics
curl http://localhost:8080/metrics

# Check Prometheus targets
curl http://localhost:9090/api/v1/targets

# View container logs
docker-compose logs -f api
docker-compose logs -f prometheus
docker-compose logs -f grafana
```

## Performance Impact

### Baseline Measurements
- **Logging**: < 1% CPU overhead
- **Metrics**: < 2% CPU overhead  
- **Tracing**: < 2% CPU overhead (with 10% sampling)
- **Total**: < 5% combined overhead

### Optimization Settings
- Use JSON logging for production
- Configure appropriate sampling rates
- Enable log filtering for noisy endpoints
- Use async logging where possible

## Security Considerations

### Sensitive Data Protection
- Automatic PII masking in logs
- Exclude sensitive headers from traces
- Secure dashboard access with authentication
- Network security for observability endpoints

### Access Control
- Grafana: Role-based access control
- Prometheus: Network-level restrictions
- Jaeger: Query-only access for users

## Migration Guide

### From Basic Logging
1. Update log statements to use structured format
2. Add correlation IDs to requests
3. Configure log aggregation
4. Set up log retention policies

### Adding Metrics
1. Install OpenTelemetry packages
2. Configure metrics collection
3. Add business metrics
4. Set up Prometheus and Grafana

### Implementing Tracing
1. Add tracing instrumentation
2. Configure trace exporters
3. Set up Jaeger
4. Implement custom spans

This observability implementation provides comprehensive monitoring and debugging capabilities while maintaining performance and following industry best practices.