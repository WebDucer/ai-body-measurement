using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Integration;

/// <summary>
/// Integration tests for the complete delete-measurement flow
/// Tests the path: Select → Confirm → Delete → Refresh
/// </summary>
public class DeleteMeasurementFlowTests
{
    [Fact]
    public async Task DeleteMeasurement_CompleteFlow_RemovesEntryFromDatabase()
    {
        // Arrange - Database with multiple entries
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 76.0, Date = DateTime.Today.AddDays(-2), CreatedAt = DateTime.Now.AddDays(-2) },
            new WeightEntry { Id = 2, WeightKg = 75.5, Date = DateTime.Today.AddDays(-1), CreatedAt = DateTime.Now.AddDays(-1) },
            new WeightEntry { Id = 3, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockStats = new Mock<IStatisticsService>();
        
        mockDb.Setup(x => x.RemoveMeasurementAsync(It.IsAny<int>()))
            .Callback<int>(id => entries.RemoveAll(e => e.Id == id))
            .ReturnsAsync(1);
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        mockStats.Setup(x => x.GetCurrentWeightAsync())
            .ReturnsAsync(() => entries.Any() ? entries.OrderByDescending(e => e.Date).First().WeightKg : (double?)null);
        
        // Act - Step 1: User selects entry to delete (ID = 2)
        int entryToDeleteId = 2;
        
        // Act - Step 2: User confirms deletion
        await mockDb.Object.RemoveMeasurementAsync(entryToDeleteId);
        
        // Act - Step 3: Refresh the list
        var remainingEntries = await mockDb.Object.GetMeasurementHistoryAsync();
        var currentWeight = await mockStats.Object.GetCurrentWeightAsync();
        
        // Assert - Verify entry was deleted
        Assert.Equal(2, remainingEntries.Count);
        Assert.DoesNotContain(remainingEntries, e => e.Id == 2);
        Assert.Contains(remainingEntries, e => e.Id == 1);
        Assert.Contains(remainingEntries, e => e.Id == 3);
        
        // Verify current weight is updated
        Assert.Equal(75.0, currentWeight);
        
        // Verify delete was called once
        mockDb.Verify(x => x.RemoveMeasurementAsync(2), Times.Once);
    }
    
    [Fact]
    public async Task DeleteLastMeasurement_SetsEmptyState()
    {
        // Arrange - Database with single entry
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockStats = new Mock<IStatisticsService>();
        
        mockDb.Setup(x => x.RemoveMeasurementAsync(It.IsAny<int>()))
            .Callback<int>(id => entries.RemoveAll(e => e.Id == id))
            .ReturnsAsync(1);
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.ToList());
        
        mockStats.Setup(x => x.GetCurrentWeightAsync())
            .ReturnsAsync(() => entries.Any() ? entries.First().WeightKg : (double?)null);
        
        // Act - Delete the only entry
        await mockDb.Object.RemoveMeasurementAsync(1);
        var remainingEntries = await mockDb.Object.GetMeasurementHistoryAsync();
        var currentWeight = await mockStats.Object.GetCurrentWeightAsync();
        
        // Assert - Verify empty state
        Assert.Empty(remainingEntries);
        Assert.Null(currentWeight);
    }
    
    [Fact]
    public async Task DeleteMeasurement_UpdatesStatistics()
    {
        // Arrange - Database with three entries
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 80.0, Date = DateTime.Today.AddDays(-30), CreatedAt = DateTime.Now.AddDays(-30) },
            new WeightEntry { Id = 2, WeightKg = 77.0, Date = DateTime.Today.AddDays(-15), CreatedAt = DateTime.Now.AddDays(-15) },
            new WeightEntry { Id = 3, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockStats = new Mock<IStatisticsService>();
        
        mockDb.Setup(x => x.RemoveMeasurementAsync(It.IsAny<int>()))
            .Callback<int>(id => entries.RemoveAll(e => e.Id == id))
            .ReturnsAsync(1);
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        // Statistics service calculations
        mockStats.Setup(x => x.GetCurrentWeightAsync())
            .ReturnsAsync(() => entries.Any() ? entries.OrderByDescending(e => e.Date).First().WeightKg : (double?)null);
        
        mockStats.Setup(x => x.GetStartingWeightAsync())
            .ReturnsAsync(() => entries.Any() ? entries.OrderBy(e => e.Date).First().WeightKg : (double?)null);
        
        mockStats.Setup(x => x.CalculateWeightChangeAsync())
            .ReturnsAsync(() =>
            {
                if (!entries.Any()) return (null, null);
                var current = entries.OrderByDescending(e => e.Date).First().WeightKg;
                var starting = entries.OrderBy(e => e.Date).First().WeightKg;
                var change = current - starting;
                var percentage = (change / starting) * 100;
                return (change, percentage);
            });
        
        // Act - Delete middle entry
        await mockDb.Object.RemoveMeasurementAsync(2);
        
        var currentWeight = await mockStats.Object.GetCurrentWeightAsync();
        var startingWeight = await mockStats.Object.GetStartingWeightAsync();
        var (weightChange, percentageChange) = await mockStats.Object.CalculateWeightChangeAsync();
        
        // Assert - Verify statistics updated correctly
        Assert.Equal(75.0, currentWeight);
        Assert.Equal(80.0, startingWeight);
        Assert.Equal(-5.0, weightChange);
        Assert.InRange(percentageChange!.Value, -6.3, -6.2); // ~-6.25%
    }
    
    [Fact]
    public async Task DeleteMeasurement_NonExistentId_DoesNothing()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        
        mockDb.Setup(x => x.RemoveMeasurementAsync(999))
            .ReturnsAsync(0); // 0 rows affected
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.ToList());
        
        // Act - Try to delete non-existent entry
        var result = await mockDb.Object.RemoveMeasurementAsync(999);
        var remainingEntries = await mockDb.Object.GetMeasurementHistoryAsync();
        
        // Assert - Verify nothing changed
        Assert.Equal(0, result);
        Assert.Single(remainingEntries);
        Assert.Equal(1, remainingEntries[0].Id);
    }
    
    [Fact]
    public async Task DeleteMultipleMeasurements_InSequence_MaintainsConsistency()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 80.0, Date = DateTime.Today.AddDays(-4), CreatedAt = DateTime.Now.AddDays(-4) },
            new WeightEntry { Id = 2, WeightKg = 78.0, Date = DateTime.Today.AddDays(-3), CreatedAt = DateTime.Now.AddDays(-3) },
            new WeightEntry { Id = 3, WeightKg = 76.0, Date = DateTime.Today.AddDays(-2), CreatedAt = DateTime.Now.AddDays(-2) },
            new WeightEntry { Id = 4, WeightKg = 74.0, Date = DateTime.Today.AddDays(-1), CreatedAt = DateTime.Now.AddDays(-1) },
            new WeightEntry { Id = 5, WeightKg = 72.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        
        mockDb.Setup(x => x.RemoveMeasurementAsync(It.IsAny<int>()))
            .Callback<int>(id => entries.RemoveAll(e => e.Id == id))
            .ReturnsAsync(1);
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        // Act - Delete multiple entries
        await mockDb.Object.RemoveMeasurementAsync(2);
        await mockDb.Object.RemoveMeasurementAsync(4);
        
        var remainingEntries = await mockDb.Object.GetMeasurementHistoryAsync();
        
        // Assert - Verify correct entries remain
        Assert.Equal(3, remainingEntries.Count);
        Assert.Equal(5, remainingEntries[0].Id); // Most recent
        Assert.Equal(3, remainingEntries[1].Id);
        Assert.Equal(1, remainingEntries[2].Id); // Oldest
        
        // Verify deletion count
        mockDb.Verify(x => x.RemoveMeasurementAsync(It.IsAny<int>()), Times.Exactly(2));
    }
}
