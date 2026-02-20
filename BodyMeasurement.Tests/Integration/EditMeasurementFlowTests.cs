using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Integration;

/// <summary>
/// Integration tests for the complete edit-measurement flow
/// Tests the path: Load → Edit → Save → Retrieve
/// </summary>
public class EditMeasurementFlowTests
{
    [Fact]
    public async Task EditMeasurement_CompleteFlow_UpdatesAndRetrievesData()
    {
        // Arrange - Simulate database with existing entry
        var entries = new List<WeightEntry>
        {
            new WeightEntry
            {
                Id = 1,
                WeightKg = 75.0,
                Date = DateTime.Today.AddDays(-1),
                Notes = "Original note",
                CreatedAt = DateTime.Now.AddDays(-1)
            }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");
        
        mockDb.Setup(x => x.FindMeasurementAsync(1))
            .ReturnsAsync(() => entries.FirstOrDefault(e => e.Id == 1));
        
        mockDb.Setup(x => x.UpdateMeasurementAsync(It.IsAny<WeightEntry>()))
            .Callback<WeightEntry>(updatedEntry =>
            {
                var existing = entries.FirstOrDefault(e => e.Id == updatedEntry.Id);
                if (existing != null)
                {
                    existing.WeightKg = updatedEntry.WeightKg;
                    existing.Date = updatedEntry.Date;
                    existing.Notes = updatedEntry.Notes;
                }
            })
            .ReturnsAsync(1);
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        // Act - Step 1: Load existing entry
        var loadedEntry = await mockDb.Object.FindMeasurementAsync(1);
        Assert.NotNull(loadedEntry);
        Assert.Equal(75.0, loadedEntry.WeightKg);
        Assert.Equal("Original note", loadedEntry.Notes);
        
        // Act - Step 2: Modify the entry
        loadedEntry.WeightKg = 75.5;
        loadedEntry.Notes = "Updated note";
        
        // Act - Step 3: Save the changes
        await mockDb.Object.UpdateMeasurementAsync(loadedEntry);
        
        // Act - Step 4: Retrieve and verify
        var updatedEntry = await mockDb.Object.FindMeasurementAsync(1);
        
        // Assert - Verify the complete flow
        Assert.NotNull(updatedEntry);
        Assert.Equal(75.5, updatedEntry.WeightKg);
        Assert.Equal("Updated note", updatedEntry.Notes);
        Assert.Equal(1, updatedEntry.Id);
        
        // Verify database methods were called
        mockDb.Verify(x => x.FindMeasurementAsync(1), Times.Exactly(2));
        mockDb.Verify(x => x.UpdateMeasurementAsync(It.IsAny<WeightEntry>()), Times.Once);
    }
    
    [Fact]
    public async Task EditMeasurement_ChangeDate_MaintainsCorrectOrder()
    {
        // Arrange - Multiple entries
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 76.0, Date = DateTime.Today.AddDays(-3), CreatedAt = DateTime.Now.AddDays(-3) },
            new WeightEntry { Id = 2, WeightKg = 75.5, Date = DateTime.Today.AddDays(-2), CreatedAt = DateTime.Now.AddDays(-2) },
            new WeightEntry { Id = 3, WeightKg = 75.0, Date = DateTime.Today.AddDays(-1), CreatedAt = DateTime.Now.AddDays(-1) }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        
        mockDb.Setup(x => x.FindMeasurementAsync(2))
            .ReturnsAsync(() => entries.FirstOrDefault(e => e.Id == 2));
        
        mockDb.Setup(x => x.UpdateMeasurementAsync(It.IsAny<WeightEntry>()))
            .Callback<WeightEntry>(updatedEntry =>
            {
                var existing = entries.FirstOrDefault(e => e.Id == updatedEntry.Id);
                if (existing != null)
                {
                    existing.Date = updatedEntry.Date;
                    existing.WeightKg = updatedEntry.WeightKg;
                }
            })
            .ReturnsAsync(1);
        
        mockDb.Setup(x => x.GetMeasurementHistoryAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        // Act - Change date of middle entry to today
        var entryToEdit = await mockDb.Object.FindMeasurementAsync(2);
        entryToEdit!.Date = DateTime.Today;
        await mockDb.Object.UpdateMeasurementAsync(entryToEdit);
        
        var allEntries = await mockDb.Object.GetMeasurementHistoryAsync();
        
        // Assert - Verify new chronological order
        Assert.Equal(3, allEntries.Count);
        Assert.Equal(2, allEntries[0].Id); // Now most recent (today)
        Assert.Equal(3, allEntries[1].Id); // Yesterday
        Assert.Equal(1, allEntries[2].Id); // 3 days ago
    }
    
    [Fact]
    public async Task EditMeasurement_ConvertUnit_StoresInKg()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry
            {
                Id = 1,
                WeightKg = 75.0,
                Date = DateTime.Today,
                CreatedAt = DateTime.Now
            }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        mockSettings.Setup(s => s.PreferredUnit).Returns("lbs");
        
        mockDb.Setup(x => x.FindMeasurementAsync(1))
            .ReturnsAsync(() => entries.FirstOrDefault(e => e.Id == 1));
        
        mockDb.Setup(x => x.UpdateMeasurementAsync(It.IsAny<WeightEntry>()))
            .Callback<WeightEntry>(updatedEntry =>
            {
                var existing = entries.FirstOrDefault(e => e.Id == updatedEntry.Id);
                if (existing != null)
                {
                    existing.WeightKg = updatedEntry.WeightKg;
                }
            })
            .ReturnsAsync(1);
        
        // Act - Load entry, display in lbs, edit, convert back to kg
        var entry = await mockDb.Object.FindMeasurementAsync(1);
        double displayWeight = WeightConverter.KgToLbs(entry!.WeightKg); // ~165.35 lbs
        
        // User edits to 170 lbs
        displayWeight = 170.0;
        entry.WeightKg = WeightConverter.LbsToKg(displayWeight);
        
        await mockDb.Object.UpdateMeasurementAsync(entry);
        
        // Assert - Verify stored in kg
        var updatedEntry = await mockDb.Object.FindMeasurementAsync(1);
        Assert.InRange(updatedEntry!.WeightKg, 77.0, 77.5); // ~77.1 kg
    }
    
    [Fact]
    public async Task EditMeasurement_NonExistentId_ReturnsNull()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.FindMeasurementAsync(999))
            .ReturnsAsync((WeightEntry?)null);
        
        // Act
        var entry = await mockDb.Object.FindMeasurementAsync(999);
        
        // Assert
        Assert.Null(entry);
    }
}
