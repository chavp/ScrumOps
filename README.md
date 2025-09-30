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

### 👥 Team Collaboration
- Real-time sprint progress visibility
- Comments and discussions on backlog items
- Event notifications (sprint start, impediments, blockers)
- Complete audit trail of all changes
- Cross-team coordination capabilities

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQLite (included)
- Modern web browser
- Git

### Quick Setup
```bash
# Clone the repository
git clone https://github.com/your-org/ScrumOps.git
cd ScrumOps

# Run the application
dotnet run --project src/ScrumOps.Web

# Navigate to https://localhost:5001
```

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

## 🏛️ Architecture

**Technology Stack**: 
- Frontend: Blazor Server/WebAssembly
- Backend: ASP.NET Core Web API
- Database: SQLite with Entity Framework Core
- Architecture: Domain Driven Design with Clean Architecture

**Key Principles**:
- Domain Driven Design (DDD) with bounded contexts
- Test-Driven Development (TDD)
- SOLID principles and clean code practices
- Event-driven architecture for cross-context communication
- Responsive design for mobile and desktop access

## 🤝 Contributing

This project follows constitutional requirements for code quality, testing standards, and user experience consistency. Please refer to the [specification documents](./specs/001-scrum-framework/) for detailed development guidelines.

## 📄 License

This project is based on the Scrum Primer 2.0 framework and implements the official Scrum Guide principles.

---

**Built with ❤️ for Agile teams everywhere**