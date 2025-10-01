# ScrumOps Infrastructure Tests

Comprehensive test suite for the ScrumOps Infrastructure layer, ensuring Entity Framework mappings, repositories, and database operations work correctly.

## 🚀 Quick Start

```bash
# Run all tests
dotnet test

# Run with custom script (includes Docker checks)
.\run-tests.ps1

# Skip integration tests (faster)
.\run-tests.ps1 -SkipIntegration

# Run specific test category
.\run-tests.ps1 -TestFilter "Repository"

# Run with code coverage
.\run-tests.ps1 -Coverage
```

## 📁 Test Structure

```
Infrastructure.Tests/
├── Builders/                    # Test data builders
│   ├── TeamBuilder.cs
│   ├── UserBuilder.cs
│   └── ProductBacklogBuilder.cs
├── Persistence/                 # EF Core and database tests
│   ├── ScrumOpsDbContextTests.cs
│   ├── EntityConfigurationTests.cs
│   └── UnitOfWorkTests.cs
├── Repositories/                # Repository pattern tests
│   ├── TeamRepositoryTests.cs
│   └── ProductBacklogRepositoryTests.cs
├── Integration/                 # Real PostgreSQL tests
│   └── PostgreSqlIntegrationTests.cs
├── TestBase.cs                  # Common test infrastructure
└── README.md
```

## 🧪 Test Categories

### **Unit Tests** (Fast - In-Memory Database)
- **Entity Persistence**: Save/retrieve domain entities
- **Value Object Mapping**: EF Core owned entity configurations
- **Repository Operations**: CRUD and query operations
- **Transaction Management**: Unit of Work pattern testing

### **Integration Tests** (Realistic - Real PostgreSQL)
- **Schema Creation**: Migration and database structure validation
- **Constraint Enforcement**: Unique constraints and foreign keys
- **Performance Testing**: Index effectiveness with large datasets
- **Container Management**: Testcontainers with PostgreSQL

## 🛠️ Test Infrastructure

### **TestBase Class**
Provides common infrastructure for all tests:
- In-memory EF Core context creation
- Automatic database setup and cleanup
- Utility methods for fresh contexts
- Proper resource disposal

### **Builder Pattern**
Type-safe test data creation:
```csharp
var team = TeamBuilder.Random();
var users = UserBuilder.CreateTeamMembers(team.Id, developerCount: 3);
var backlog = ProductBacklogBuilder.Random(team.Id);
var items = ProductBacklogItemBuilder.CreateMultiple(backlog.Id, 5);
```

### **FluentAssertions**
Readable and maintainable assertions:
```csharp
result.Should().NotBeNull();
result.Name.Value.Should().Be("Expected Name");
items.Should().HaveCount(5);
teams.Should().AllSatisfy(t => t.IsActive.Should().BeTrue());
```

## 📊 Coverage Areas

### **Entity Framework Mappings**
- ✅ All domain entities (Team, User, ProductBacklog, etc.)
- ✅ Value object configurations (TeamName, Email, ScrumRole, etc.)
- ✅ Relationship mappings with navigation properties
- ✅ Index and constraint configurations

### **Repository Pattern**
- ✅ **TeamRepository**: CRUD operations, active team filtering, name searches
- ✅ **ProductBacklogRepository**: Team associations, item management, status filtering
- ✅ All async operations with proper cancellation token support
- ✅ Include statements for loading related entities

### **Database Operations**
- ✅ **Transactions**: Begin, commit, rollback with Unit of Work pattern
- ✅ **Constraints**: Unique constraints and foreign key enforcement
- ✅ **Schema Organization**: Proper bounded context separation
- ✅ **Performance**: Index effectiveness and query optimization

## 🐳 Integration Testing

Uses **Testcontainers** for real PostgreSQL integration tests:

```csharp
[Fact]
public async Task CanCreateComplexEntityGraph()
{
    // Creates real PostgreSQL container
    // Tests actual database constraints
    // Validates migration compatibility
    // Measures query performance
}
```

**Container Features**:
- Automatic PostgreSQL container lifecycle
- Migration execution in real database
- Constraint validation with actual database engine
- Performance testing with realistic data volumes

## ⚡ Performance Considerations

### **Test Execution Speed**
- **Unit Tests**: ~2-3 seconds (in-memory database)
- **Integration Tests**: ~10-15 seconds (includes container startup)
- **Total Suite**: ~20 seconds for complete run

### **Resource Management**
- In-memory databases for fast unit tests
- Container reuse for integration test efficiency
- Proper disposal patterns for memory management
- Change tracking optimization for test isolation

## 🔧 Development Workflow

### **TDD Support**
```bash
# Watch mode for rapid feedback
dotnet watch test --filter "YourTestName"

# Specific test category during development
dotnet test --filter "Category=Repository"
```

### **Debugging Tests**
```csharp
// TestBase provides debug-friendly contexts
var context = CreateFreshContext(); // New context for isolation
await SaveAndDetachAsync();         // Clear change tracking
```

### **Adding New Tests**
1. Use existing builders for test data
2. Follow Arrange-Act-Assert pattern
3. Test both success and error scenarios
4. Add integration tests for complex operations

## 📋 Test Maintenance

### **Updating Tests for Domain Changes**
1. Update relevant builders in `Builders/` folder
2. Add new entity configurations tests
3. Update repository tests for new operations
4. Add integration tests for new constraints

### **Performance Regression Detection**
Integration tests include performance assertions:
```csharp
stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
```

### **Test Data Management**
- Builders use valid business rules automatically
- Random data generation with deterministic seeds
- Relationship management across entity boundaries
- Proper value object instantiation with validation

## ✅ Quality Assurance

### **Test Reliability**
- Each test uses isolated database context
- No shared state between tests
- Deterministic results with proper cleanup
- Exception scenarios thoroughly tested

### **Maintainability**
- Clear test method naming describing exact scenarios
- Consistent Arrange-Act-Assert structure
- Reusable test data creation with builders
- Shared infrastructure through TestBase

---

## 🎯 Business Value

This comprehensive test suite ensures:
- **Database changes are validated** before deployment
- **Refactoring is safe** with full regression protection
- **New features work correctly** with existing functionality
- **Performance regressions are caught** early in development
- **Production reliability** through real database compatibility testing

The Infrastructure Tests provide confidence that the data access layer is robust, reliable, and ready for production deployment.