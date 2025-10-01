using ScrumOps.Infrastructure.Persistence;
using ScrumOps.Infrastructure.Tests.Builders;

namespace ScrumOps.Infrastructure.Tests.Persistence;

/// <summary>
/// Tests for UnitOfWork transaction management.
/// </summary>
public class UnitOfWorkTests : TestBase
{
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _unitOfWork = new UnitOfWork(Context);
    }

    [Fact]
    public async Task SaveChangesAsync_CommitsAllChanges()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var user = UserBuilder.Random(team.Id);

        Context.Teams.Add(team);
        Context.Users.Add(user);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(2); // 2 entities saved
        
        Context.ChangeTracker.Clear();
        
        var savedTeam = await Context.Teams.FindAsync(team.Id);
        var savedUser = await Context.Users.FindAsync(user.Id);
        
        savedTeam.Should().NotBeNull();
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_RespectsToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var team = TeamBuilder.Random();
        Context.Teams.Add(team);

        // Act
        cts.Cancel();
        var act = async () => await _unitOfWork.SaveChangesAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task BeginTransactionAsync_CreatesTransaction()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        Context.Database.CurrentTransaction.Should().NotBeNull();
    }

    [Fact]
    public async Task Transaction_CanCommitChanges()
    {
        // Arrange
        var team = TeamBuilder.Random();
        
        // Act
        await _unitOfWork.BeginTransactionAsync();
        
        Context.Teams.Add(team);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        Context.ChangeTracker.Clear();
        var savedTeam = await Context.Teams.FindAsync(team.Id);
        savedTeam.Should().NotBeNull();
    }

    [Fact]
    public async Task Transaction_CanRollbackChanges()
    {
        // Arrange
        var team = TeamBuilder.Random();
        
        // Act
        await _unitOfWork.BeginTransactionAsync();
        
        Context.Teams.Add(team);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        Context.ChangeTracker.Clear();
        var savedTeam = await Context.Teams.FindAsync(team.Id);
        savedTeam.Should().BeNull();
    }

    [Fact]
    public async Task MultipleOperations_InSingleTransaction_AreAtomic()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var backlog = ProductBacklogBuilder.Random(team.Id);
        var items = ProductBacklogItemBuilder.CreateMultiple(backlog.Id, 3);

        // Act
        await _unitOfWork.BeginTransactionAsync();
        
        Context.Teams.Add(team);
        Context.ProductBacklogs.Add(backlog);
        Context.ProductBacklogItems.AddRange(items);
        
        var changeCount = await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        changeCount.Should().Be(5); // 1 team + 1 backlog + 3 items
        
        Context.ChangeTracker.Clear();
        
        var savedTeam = await Context.Teams.FindAsync(team.Id);
        var savedBacklog = await Context.ProductBacklogs.FindAsync(backlog.Id);
        var savedItems = Context.ProductBacklogItems.Where(i => i.ProductBacklogId == backlog.Id).ToList();
        
        savedTeam.Should().NotBeNull();
        savedBacklog.Should().NotBeNull();
        savedItems.Should().HaveCount(3);
    }

    [Fact]
    public async Task FailedTransaction_RollsBackAllChanges()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var duplicateTeam = new TeamBuilder().WithName(team.Name.Value).Build(); // Same name will cause constraint violation

        // Act & Assert
        await _unitOfWork.BeginTransactionAsync();
        
        Context.Teams.Add(team);
        await _unitOfWork.SaveChangesAsync();
        
        Context.Teams.Add(duplicateTeam);
        
        var act = async () => await _unitOfWork.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
        
        await _unitOfWork.RollbackTransactionAsync();
        
        // Verify rollback
        Context.ChangeTracker.Clear();
        var savedTeam = await Context.Teams.FindAsync(team.Id);
        savedTeam.Should().BeNull();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // UnitOfWork doesn't implement IDisposable in current implementation
        }
        base.Dispose(disposing);
    }
}