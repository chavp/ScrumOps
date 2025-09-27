# Next Priority Actions

Based on the current codebase analysis, here are the immediate priority actions:

## 🔥 URGENT - Fix Existing TODOs (1-2 hours)

### 1. ProductBacklogController.cs - Team Name Resolution
**Files**: `src/ScrumOps.Api/Controllers/ProductBacklogController.cs`
**Lines**: 63, 120, 177
**Issue**: Hard-coded team names using `$"Team {teamId}"` instead of actual team names
**Fix**: Call team service to get actual team names or use existing team data

### 2. TeamsController.cs - Missing Query Implementations  
**File**: `src/ScrumOps.Api/Controllers/TeamsController.cs`
**Lines**: 110, 179, 231
**Issues**:
- GetTeamQuery and handler not implemented
- UpdateTeamCommand and handler not implemented  
- GetTeamsQuery and handler not implemented
**Fix**: Implement the missing CQRS queries and commands

## 🟡 HIGH PRIORITY - Complete Application Layer (2-3 hours)

### 3. Team Management Queries
**Missing**:
- `src/ScrumOps.Application/TeamManagement/Queries/GetTeamQuery.cs`
- `src/ScrumOps.Application/TeamManagement/Queries/GetTeamsQuery.cs`
- `src/ScrumOps.Application/TeamManagement/Handlers/QueryHandlers/GetTeamQueryHandler.cs`
- `src/ScrumOps.Application/TeamManagement/Handlers/QueryHandlers/GetTeamsQueryHandler.cs`

### 4. Sprint Management Use Cases
**Missing Complete Sprint Application Layer**:
- Commands: CreateSprint, UpdateSprint, StartSprint, CompleteSprint
- Queries: GetSprint, GetSprints, GetSprintBacklog
- Handlers for all commands and queries
- DTOs for Sprint operations

## 🟢 MEDIUM PRIORITY - Infrastructure Completion (2-3 hours)

### 5. Sprint Repository Implementation
**Missing**: `src/ScrumOps.Infrastructure/Persistence/Repositories/SprintRepository.cs`

### 6. Database Migrations and Seed Data
**Missing**:
- Initial EF Core migration
- Seed data configuration for development/testing

## 🔵 LOWER PRIORITY - UI Components (4-6 hours)

### 7. Blazor Components Implementation
**Status**: No Razor components implemented yet
**Need**: All 15 UI tasks from Phase 3.7

## 🚦 CRITICAL PATH

```
Fix TODOs (T103-T106) 
    ↓
Complete Team Queries (T111-T112)
    ↓
Implement Sprint Application Layer (T113-T116)  
    ↓
Add Sprint Repository (T117)
    ↓
Create SprintsController (T107)
    ↓
Add Missing Tests (T109-T110)
    ↓
Implement UI Components (T120-T128)
```

## ⚡ Quick Wins (30-60 minutes each)

1. **T104**: Add GetTeamQuery + Handler (extend existing team infrastructure)
2. **T105**: Add UpdateTeamCommand + Handler (extend existing team infrastructure)  
3. **T106**: Add GetTeamsQuery + Handler (extend existing team infrastructure)
4. **T103**: Fix team name resolution in ProductBacklogController (call existing team service)
5. **T129**: Fix user context in AddBacklogItemCommandHandler
6. **T084**: Add Priority component for ProductBacklog UI
7. **T078**: Add common form components for Web UI

## 📊 Current Completion Status

- **Domain Layer**: ✅ 100% Complete (23/23)
- **Application Layer**: 🟡 37% Complete (7/19) 
- **Infrastructure Layer**: 🟡 58% Complete (7/12)
- **API Layer**: 🟡 33% Complete (2/6) 
- **UI Layer**: 🟢 80% Complete (12/15) - **MAJOR PROGRESS!**
- **Outstanding TODOs**: 🔴 0% Complete (0/29)

**🎉 MAJOR ACHIEVEMENT**: Blazor UI components are nearly complete! All main pages and components implemented.

**Next Session Goal**: Complete Phase 3.9 outstanding items (T103-T131) to reach 65% overall completion and functional system.