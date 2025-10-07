# Observability Implementation Prompt

## Context
I need to add comprehensive observability features to my application to improve monitoring, debugging, and performance analysis. The implementation should follow best practices and industry standards.

## Requirements

### 1. Logging
Implement structured logging with the following capabilities:
- **Log Levels**: Support for DEBUG, INFO, WARN, ERROR, and FATAL levels
- **Structured Format**: Use JSON format for machine readability
- **Contextual Information**: Include timestamp, service name, trace ID, span ID, user ID, and request ID
- **Log Rotation**: Implement file rotation based on size and time
- **Sensitive Data**: Mask or redact sensitive information (passwords, tokens, PII)
- **Correlation**: Enable correlation between logs and traces using trace IDs

### 2. Metrics
Implement metrics collection covering:
- **Application Metrics**:
  - Request rate (requests per second)
  - Error rate and error types
  - Response time (latency percentiles: p50, p95, p99)
  - Success rate
- **System Metrics**:
  - CPU usage
  - Memory usage
  - Disk I/O
  - Network I/O
- **Business Metrics**:
  - Custom counters for business events
  - Gauges for current state values
  - Histograms for distributions
- **Format**: Use Prometheus exposition format or OpenTelemetry standards
- **Cardinality**: Avoid high-cardinality labels

### 3. Distributed Tracing
Implement distributed tracing with:
- **Trace Context Propagation**: Use W3C Trace Context or OpenTelemetry standards
- **Span Creation**: Automatically instrument HTTP requests, database calls, external API calls
- **Custom Spans**: Support for manual span creation for business logic
- **Span Attributes**: Include relevant metadata (HTTP method, status code, database query, etc.)
- **Sampling**: Implement intelligent sampling (head-based or tail-based)
- **Trace Visualization**: Export to systems like Jaeger, Zipkin, or cloud providers

## Technical Specifications

### Technology Stack
Please specify or use the following:
- **Programming Language**: [Your language, e.g., Python, Java, Go, Node.js]
- **Framework**: [Your framework, e.g., Spring Boot, Express, FastAPI]
- **Observability Library**: OpenTelemetry (preferred) or language-specific alternatives
- **Backend Systems**:
  - Logs: Elasticsearch, Loki, or CloudWatch Logs
  - Metrics: Prometheus, Grafana Cloud, or CloudWatch Metrics
  - Traces: Jaeger, Tempo, or AWS X-Ray

### Implementation Guidelines
1. **Minimal Performance Impact**: Ensure observability adds <5% overhead
2. **Environment Configuration**: Support different configurations for dev, staging, and production
3. **Error Handling**: Observability failures should not crash the application
4. **Backward Compatibility**: Maintain existing functionality while adding observability
5. **Documentation**: Provide clear documentation and examples

## Expected Deliverables

1. **Code Implementation**:
   - Logging middleware/interceptor
   - Metrics collection and exposition endpoint
   - Tracing instrumentation and configuration
   - Configuration files with sensible defaults

2. **Configuration Examples**:
   - Environment-specific configurations
   - Integration with backend systems

3. **Dashboard Templates** (optional):
   - Grafana dashboards for metrics
   - Example queries for logs and traces

4. **Documentation**:
   - Setup and configuration guide
   - Best practices for adding custom metrics/logs
   - Troubleshooting guide

## Example Use Cases
- Debug production issues by correlating logs with traces
- Monitor API performance and identify slow endpoints
- Track business KPIs in real-time
- Set up alerts for error rate spikes or latency increases
- Analyze user journey through distributed services

## Additional Considerations
- [ ] Multi-tenant support (if applicable)
- [ ] Cost optimization for observability backends
- [ ] Privacy and compliance (GDPR, data residency)
- [ ] Integration with existing monitoring tools
- [ ] Support for blue-green or canary deployments

## Success Criteria
- All three pillars (logs, metrics, traces) are implemented and working
- Zero application crashes due to observability code
- Performance overhead is within acceptable limits
- Team can effectively debug production issues using the observability data
- Dashboards provide actionable insights into application health