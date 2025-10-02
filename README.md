# ScrumOps - Scrum Framework Management System

## 🎯 Product Overview

ScrumOps is a comprehensive Scrum framework management system designed to help teams implement and track Scrum processes according to the official Scrum Guide principles. Built with modern web technologies, it provides an intuitive interface for managing all aspects of Scrum methodology.

## ✨ Key Features

### 🔐 Role-Based Access Control
- **Product Owner**: Manage product backlog, set priorities, define acceptance criteria
- **Scrum Master**: Facilitate processes, track impediments, generate reports
- **Development Team**: Update task progress, log daily activities, collaborate on deliverables

### 📋 Product Backlog Management
- Create and prioritize product backlog items with clear acceptance criteria
- Drag-and-drop prioritization based on business value
- Backlog refinement with estimates, dependencies, and detailed descriptions
- Complete lifecycle tracking (New → Ready → In Progress → Done)
- Archive completed or obsolete items

### 🏃‍♂️ Sprint Management
- Configurable sprint lengths (1-4 weeks per team)
- Sprint planning with capacity-based item selection
- Clear sprint goal definition and tracking
- Protected sprint scope with proper change management
- Sprint backlog creation from prioritized items

### 📅 Daily Scrum Support
- Daily progress updates with three key questions tracking
- Impediment logging and resolution tracking
- Real-time burndown charts and progress visualization
- Team coordination and transparency tools
- Mobile-friendly daily standup interface

### 🎉 Sprint Events
- **Sprint Planning**: Select items, define goals, estimate capacity
- **Daily Scrum**: 15-minute time-boxed coordination meetings
- **Sprint Review**: Stakeholder demos and feedback collection
- **Sprint Retrospective**: Structured improvement identification (What Went Well, What Could Improve, Action Items)

### 📊 Metrics & Reporting
- Team velocity tracking over time
- Sprint and release burndown charts
- Key Scrum metrics (cycle time, lead time, throughput)
- Exportable reports for stakeholder communication
- Performance insights for continuous improvement

### 🔍 Observability & Monitoring
- **Structured Logging**: JSON-formatted logs with correlation IDs, log rotation, and sensitive data masking
- **Metrics Collection**: Application, system, and business metrics via OpenTelemetry and Prometheus
- **Distributed Tracing**: Request tracing across services with W3C Trace Context standards
- **Health Monitoring**: Comprehensive health checks for application readiness and liveness
- **Performance Tracking**: Real-time monitoring of API response times, error rates, and system resources
- **Business Intelligence**: Custom metrics for sprint completion rates, team velocity, and backlog progress

### 👥 Team Collaboration
- Real-time sprint progress visibility
- Comments and discussions on backlog items
- Event notifications (sprint start, impediments, blockers)
- Complete audit trail of all changes
- Cross-team coordination capabilities

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Docker Desktop (for containerized deployment)
- PostgreSQL 16+ (automatically managed via Docker)
- Modern web browser (Chrome, Firefox, Edge)
- Git

### Observability Stack (Included in Docker Compose)
- **Prometheus**: Metrics collection and storage
- **Grafana**: Visualization dashboards and alerting
- **Jaeger**: Distributed tracing and performance monitoring
- **Serilog**: Structured logging with JSON output

### Quick Setup

#### Option 1: Docker (Recommended)
```bash
# Clone the repository
git clone https://github.com/your-org/ScrumOps.git
cd ScrumOps

# Test the setup (optional - includes EF migration check)
.\test-ef-migration.ps1

# Test Docker setup
.\test-docker.ps1

# Run with Docker Compose (includes PostgreSQL database + observability stack)
docker-compose up -d

# View logs
docker-compose logs -f

# The API will be available at http://localhost:8080
# Complete observability stack:
# - Grafana dashboards: http://localhost:3000 (admin/admin123)
# - Prometheus metrics: http://localhost:9090
# - Jaeger tracing: http://localhost:16686
# - pgAdmin: http://localhost:8081 (admin@scrumops.com / admin123)
```

#### Option 2: Local Development with EF Migrations
```bash
# Clone the repository
git clone https://github.com/your-org/ScrumOps.git
cd ScrumOps

# Start PostgreSQL (if not using Docker)
docker-compose up -d postgres

# Apply Entity Framework migrations
dotnet ef database update --project src/ScrumOps.Infrastructure --startup-project src/ScrumOps.Api

# Run the application
dotnet run --project src/ScrumOps.Api

# Navigate to https://localhost:5001
```

### Service URLs
- **API**: http://localhost:8080
- **Health Check**: http://localhost:8080/health  
- **Metrics Endpoint**: http://localhost:8080/metrics
- **Swagger Documentation**: http://localhost:8080/swagger
- **Grafana Dashboards**: http://localhost:3000 (admin/admin123)
- **Prometheus**: http://localhost:9090
- **Jaeger Tracing**: http://localhost:16686
- **pgAdmin**: http://localhost:8081 (admin@scrumops.com / admin123)
- **PostgreSQL**: localhost:5433 (scrumops / scrumops123)

## 📚 Documentation Index

### 📋 Feature Specification (001-scrum-framework)
- **[Main Specification](./specs/001-scrum-framework/spec.md)** - Complete feature requirements and user scenarios
- **[Reference Guide](./specs/001-scrum-framework/REFERENCE.md)** - Scrum Primer framework reference
- **[Status Updates](./specs/001-scrum-framework/status-update.md)** - Current implementation status
- **[Next Actions](./specs/001-scrum-framework/next-actions.md)** - Upcoming development priorities

### 🏗️ Implementation Plan
- **[Master Plan](./specs/001-scrum-framework/plan/plan.md)** - Technical architecture and implementation approach
- **[Data Model](./specs/001-scrum-framework/plan/data-model.md)** - Domain entities and database design
- **[Research](./specs/001-scrum-framework/plan/research.md)** - Technology decisions and DDD patterns
- **[Quickstart Guide](./specs/001-scrum-framework/plan/quickstart.md)** - Development setup and testing procedures
- **[Domain Events](./specs/001-scrum-framework/plan/domain-events.md)** - Event-driven architecture design

### 🔌 API Contracts
- **[Teams API](./specs/001-scrum-framework/plan/contracts/teams-api.md)** - Team management endpoints
- **[Sprints API](./specs/001-scrum-framework/plan/contracts/sprints-api.md)** - Sprint lifecycle management
- **[Backlog API](./specs/001-scrum-framework/plan/contracts/backlog-api.md)** - Product backlog operations

### 📋 Development Tasks
- **[Implementation Tasks](./specs/001-scrum-framework/tasks.md)** - Detailed development roadmap

### 🔍 Observability Documentation
- **[Observability Guide](./docs/OBSERVABILITY.md)** - Comprehensive monitoring and observability documentation
- **[Implementation Summary](./OBSERVABILITY_IMPLEMENTATION_SUMMARY.md)** - Observability features implementation details

### ⚙️ Development Tools
- **[Copilot Instructions](./specs/001-scrum-framework/.github/copilot-instructions.md)** - AI coding assistant configuration

## 🎯 Success Criteria

### Functional Success
- ✅ Complete Scrum process cycle support
- ✅ Proper time-boxing for all Scrum events
- ✅ Meaningful team performance metrics
- ✅ Framework rule enforcement without over-restriction

### User Experience Success
- ✅ Minimal training required for new users
- ✅ Daily updates completed in <2 minutes per team member
- ✅ Actionable insights for continuous improvement
- ✅ Full mobile access capability

### Technical Success
- ✅ Multi-team scalability
- ✅ Data integrity across all operations
- ✅ Integration with existing development tools
- ✅ <200ms API response times (95th percentile)
- ✅ Comprehensive observability (logs, metrics, traces)
- ✅ Production-ready monitoring and alerting
- ✅ <5% performance overhead from observability features

## 🔍 Observability & Monitoring

ScrumOps includes enterprise-grade observability features for production monitoring, debugging, and performance analysis:

### Three Pillars of Observability

#### 📊 Structured Logging
- **JSON Format**: Machine-readable logs with contextual information
- **Correlation IDs**: Link logs with traces and requests
- **Log Levels**: DEBUG, INFO, WARN, ERROR, FATAL with environment-specific filtering
- **Rotation & Retention**: Automatic log rotation with configurable retention policies
- **Sensitive Data Protection**: Automatic masking of PII and credentials in production

#### 📈 Metrics Collection
- **Application Metrics**: Request rates, error rates, response times (p50, p95, p99)
- **Business Metrics**: Sprint completion rates, team velocity, backlog progress
- **System Metrics**: CPU, memory, and runtime performance via OpenTelemetry
- **Prometheus Format**: Industry-standard metrics export for monitoring systems

#### 🔗 Distributed Tracing
- **Request Tracing**: End-to-end request flow across services
- **Database Instrumentation**: Entity Framework Core query tracing
- **W3C Standards**: Trace context propagation following industry standards
- **Sampling**: Intelligent sampling (100% dev, 10% prod) for performance

### Monitoring Dashboards

Access comprehensive monitoring through Grafana dashboards:
- **ScrumOps Overview**: Key performance indicators and system health
- **HTTP Metrics**: Request rates, error rates, and latency percentiles
- **Business Intelligence**: Team performance and sprint analytics
- **Custom Alerts**: Configurable alerts for critical system events

### Performance Targets
- **< 5% Overhead**: Minimal performance impact from observability
- **< 200ms API Response**: 95th percentile response time target
- **99.9% Uptime**: High availability monitoring and alerting
- **Real-time Insights**: Live dashboards for immediate issue detection

For detailed setup and configuration, see the [Observability Documentation](./docs/OBSERVABILITY.md).

## 🏛️ Architecture

**Technology Stack**: 
- **Frontend**: Blazor Server/WebAssembly with responsive design
- **Backend**: ASP.NET Core 8.0 Web API with minimal APIs
- **Database**: PostgreSQL 16 with Entity Framework Core (Code First approach)
- **Containerization**: Docker & Docker Compose for all services
- **Architecture**: Domain Driven Design with Clean Architecture principles
- **Observability**: OpenTelemetry, Serilog, Prometheus, Grafana, Jaeger
- **Additional Services**: pgAdmin for database management

**Key Principles**:
- Domain Driven Design (DDD) with bounded contexts
- Test-Driven Development (TDD)
- SOLID principles and clean code practices
- Event-driven architecture for cross-context communication
- Responsive design for mobile and desktop access
- Code First Entity Framework migrations for database schema management

## 🤝 Contributing

This project follows constitutional requirements for code quality, testing standards, and user experience consistency. Please refer to the [specification documents](./specs/001-scrum-framework/) for detailed development guidelines.

## 📄 License

This project is based on the Scrum Primer 2.0 framework and implements the official Scrum Guide principles.

---

**Built with ❤️ for Agile teams everywhere**