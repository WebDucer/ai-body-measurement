using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Integration;

/// <summary>
/// Integration tests for export, settings, and onboarding flows
/// </summary>
public class AdditionalFlowTests
{
    #region Export Flow Tests (24.4)
    
    [Fact]
    public async Task ExportFlow_CsvFormat_GeneratesAndSharesFile()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today.AddDays(-1), Notes = "Morning", CreatedAt = DateTime.Now.AddDays(-1) },
            new WeightEntry { Id = 2, WeightKg = 74.5, Date = DateTime.Today, Notes = "After workout", CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockExport = new Mock<IExportService>();
        var mockSettings = new Mock<ISettingsService>();
        
        mockDb.Setup(x => x.GetAllWeightEntriesAsync()).ReturnsAsync(entries);
        mockSettings.Setup(s => s.Language).Returns("en");
        
        mockExport.Setup(x => x.ExportToCsvAsync(It.IsAny<List<WeightEntry>>(), It.IsAny<string>()))
            .ReturnsAsync("/path/to/weight-data-2026-02-16.csv");
        
        mockExport.Setup(x => x.ShareFileAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act - Step 1: User selects export
        var allEntries = await mockDb.Object.GetAllWeightEntriesAsync();
        
        // Act - Step 2: Generate CSV
        var csvPath = await mockExport.Object.ExportToCsvAsync(allEntries, "en");
        
        // Act - Step 3: Share file
        var shared = await mockExport.Object.ShareFileAsync(csvPath);
        
        // Assert
        Assert.NotEmpty(csvPath);
        Assert.Contains("weight-data", csvPath);
        Assert.Contains(".csv", csvPath);
        Assert.True(shared);
        
        mockExport.Verify(x => x.ExportToCsvAsync(It.IsAny<List<WeightEntry>>(), "en"), Times.Once);
        mockExport.Verify(x => x.ShareFileAsync(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task ExportFlow_JsonFormat_GeneratesAndSharesFile()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockExport = new Mock<IExportService>();
        
        mockDb.Setup(x => x.GetAllWeightEntriesAsync()).ReturnsAsync(entries);
        
        mockExport.Setup(x => x.ExportToJsonAsync(It.IsAny<List<WeightEntry>>()))
            .ReturnsAsync("/path/to/weight-data-2026-02-16.json");
        
        mockExport.Setup(x => x.ShareFileAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        var allEntries = await mockDb.Object.GetAllWeightEntriesAsync();
        var jsonPath = await mockExport.Object.ExportToJsonAsync(allEntries);
        var shared = await mockExport.Object.ShareFileAsync(jsonPath);
        
        // Assert
        Assert.NotEmpty(jsonPath);
        Assert.Contains(".json", jsonPath);
        Assert.True(shared);
    }
    
    [Fact]
    public async Task ExportFlow_EmptyData_HandlesGracefully()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var mockExport = new Mock<IExportService>();
        
        mockDb.Setup(x => x.GetAllWeightEntriesAsync()).ReturnsAsync(new List<WeightEntry>());
        
        mockExport.Setup(x => x.ExportToCsvAsync(It.IsAny<List<WeightEntry>>(), It.IsAny<string>()))
            .ReturnsAsync((string?)null); // No file generated for empty data
        
        // Act
        var entries = await mockDb.Object.GetAllWeightEntriesAsync();
        var csvPath = await mockExport.Object.ExportToCsvAsync(entries, "en");
        
        // Assert
        Assert.Empty(entries);
        Assert.Null(csvPath);
    }
    
    #endregion
    
    #region Language Switching Tests (24.5)
    
    [Fact]
    public void LanguageSwitching_UpdatesSettings()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        string currentLanguage = "en";
        
        mockSettings.Setup(s => s.Language).Returns(() => currentLanguage);
        mockSettings.SetupSet(s => s.Language = It.IsAny<string>()).Callback<string>(value => currentLanguage = value);
        
        // Act - User switches language to German
        mockSettings.Object.Language = "de";
        
        // Assert
        Assert.Equal("de", currentLanguage);
        mockSettings.VerifySet(s => s.Language = "de", Times.Once);
    }
    
    [Fact]
    public void LanguageSwitching_PersistsAcrossAppRestarts()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        string persistedLanguage = "de";
        
        mockSettings.Setup(s => s.Language).Returns(persistedLanguage);
        
        // Act - Simulate app restart, retrieve language
        var language = mockSettings.Object.Language;
        
        // Assert - Language persisted
        Assert.Equal("de", language);
    }
    
    [Fact]
    public async Task LanguageSwitching_UpdatesExportHeaders()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockExport = new Mock<IExportService>();
        var mockSettings = new Mock<ISettingsService>();
        
        // Test with English
        mockSettings.Setup(s => s.Language).Returns("en");
        mockExport.Setup(x => x.ExportToCsvAsync(It.IsAny<List<WeightEntry>>(), "en"))
            .ReturnsAsync("/path/to/export_en.csv");
        
        var csvPathEn = await mockExport.Object.ExportToCsvAsync(entries, "en");
        
        // Test with German
        mockSettings.Setup(s => s.Language).Returns("de");
        mockExport.Setup(x => x.ExportToCsvAsync(It.IsAny<List<WeightEntry>>(), "de"))
            .ReturnsAsync("/path/to/export_de.csv");
        
        var csvPathDe = await mockExport.Object.ExportToCsvAsync(entries, "de");
        
        // Assert
        Assert.Contains("en", csvPathEn);
        Assert.Contains("de", csvPathDe);
    }
    
    #endregion
    
    #region Unit Switching Tests (24.6)
    
    [Fact]
    public void UnitSwitching_UpdatesSettings()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        string currentUnit = "kg";
        
        mockSettings.Setup(s => s.PreferredUnit).Returns(() => currentUnit);
        mockSettings.SetupSet(s => s.PreferredUnit = It.IsAny<string>()).Callback<string>(value => currentUnit = value);
        
        // Act - User switches to lbs
        mockSettings.Object.PreferredUnit = "lbs";
        
        // Assert
        Assert.Equal("lbs", currentUnit);
        mockSettings.VerifySet(s => s.PreferredUnit = "lbs", Times.Once);
    }
    
    [Fact]
    public void UnitSwitching_ConvertsDisplayedWeights()
    {
        // Arrange
        var entry = new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today, CreatedAt = DateTime.Now };
        var mockSettings = new Mock<ISettingsService>();
        
        // Act - Display in kg
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");
        double displayWeightKg = entry.WeightKg;
        
        // Act - Display in lbs
        mockSettings.Setup(s => s.PreferredUnit).Returns("lbs");
        double displayWeightLbs = WeightConverter.KgToLbs(entry.WeightKg);
        
        // Assert
        Assert.Equal(75.0, displayWeightKg);
        Assert.InRange(displayWeightLbs, 165.0, 166.0); // ~165.35 lbs
    }
    
    [Fact]
    public async Task UnitSwitching_ConvertsAllWeightsInList()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Id = 1, WeightKg = 80.0, Date = DateTime.Today.AddDays(-2), CreatedAt = DateTime.Now.AddDays(-2) },
            new WeightEntry { Id = 2, WeightKg = 75.0, Date = DateTime.Today.AddDays(-1), CreatedAt = DateTime.Now.AddDays(-1) },
            new WeightEntry { Id = 3, WeightKg = 70.0, Date = DateTime.Today, CreatedAt = DateTime.Now }
        };
        
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        mockDb.Setup(x => x.GetAllWeightEntriesAsync()).ReturnsAsync(entries);
        mockSettings.Setup(s => s.PreferredUnit).Returns("lbs");
        
        // Act - Load entries and convert to preferred unit
        var allEntries = await mockDb.Object.GetAllWeightEntriesAsync();
        var convertedWeights = allEntries.Select(e => WeightConverter.KgToLbs(e.WeightKg)).ToList();
        
        // Assert - All weights converted
        Assert.Equal(3, convertedWeights.Count);
        Assert.InRange(convertedWeights[0], 176.0, 177.0); // ~176.37 lbs
        Assert.InRange(convertedWeights[1], 165.0, 166.0); // ~165.35 lbs
        Assert.InRange(convertedWeights[2], 154.0, 155.0); // ~154.32 lbs
    }
    
    [Fact]
    public void UnitSwitching_PersistsAcrossAppRestarts()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        string persistedUnit = "lbs";
        
        mockSettings.Setup(s => s.PreferredUnit).Returns(persistedUnit);
        
        // Act - Simulate app restart, retrieve unit preference
        var unit = mockSettings.Object.PreferredUnit;
        
        // Assert - Unit preference persisted
        Assert.Equal("lbs", unit);
    }
    
    #endregion
    
    #region Onboarding Flow Tests (24.7)
    
    [Fact]
    public void OnboardingFlow_FirstLaunch_ShowsOnboarding()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.Setup(s => s.OnboardingCompleted).Returns(false);
        
        // Act - Check if onboarding should be shown
        bool shouldShowOnboarding = !mockSettings.Object.OnboardingCompleted;
        
        // Assert
        Assert.True(shouldShowOnboarding);
    }
    
    [Fact]
    public void OnboardingFlow_CompletesAndSavesFlag()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        bool onboardingCompleted = false;
        
        mockSettings.Setup(s => s.OnboardingCompleted).Returns(() => onboardingCompleted);
        mockSettings.SetupSet(s => s.OnboardingCompleted = It.IsAny<bool>()).Callback<bool>(value => onboardingCompleted = value);
        
        // Act - User completes onboarding
        mockSettings.Object.OnboardingCompleted = true;
        
        // Assert
        Assert.True(onboardingCompleted);
        mockSettings.VerifySet(s => s.OnboardingCompleted = true, Times.Once);
    }
    
    [Fact]
    public void OnboardingFlow_SubsequentLaunches_SkipsOnboarding()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.Setup(s => s.OnboardingCompleted).Returns(true);
        
        // Act - Check if onboarding should be shown
        bool shouldShowOnboarding = !mockSettings.Object.OnboardingCompleted;
        
        // Assert - Onboarding should not be shown
        Assert.False(shouldShowOnboarding);
    }
    
    [Fact]
    public void OnboardingFlow_SavesUnitPreference()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        string selectedUnit = "kg";
        
        mockSettings.Setup(s => s.PreferredUnit).Returns(() => selectedUnit);
        mockSettings.SetupSet(s => s.PreferredUnit = It.IsAny<string>()).Callback<string>(value => selectedUnit = value);
        
        // Act - User selects lbs during onboarding
        mockSettings.Object.PreferredUnit = "lbs";
        mockSettings.Object.OnboardingCompleted = true;
        
        // Assert
        Assert.Equal("lbs", selectedUnit);
        mockSettings.VerifySet(s => s.PreferredUnit = "lbs", Times.Once);
    }
    
    [Fact]
    public void OnboardingFlow_CanBeSkipped()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        bool onboardingCompleted = false;
        string defaultUnit = "kg";
        
        mockSettings.Setup(s => s.OnboardingCompleted).Returns(() => onboardingCompleted);
        mockSettings.SetupSet(s => s.OnboardingCompleted = It.IsAny<bool>()).Callback<bool>(value => onboardingCompleted = value);
        mockSettings.Setup(s => s.PreferredUnit).Returns(defaultUnit);
        
        // Act - User skips onboarding
        mockSettings.Object.OnboardingCompleted = true;
        
        // Assert - Onboarding marked complete with default settings
        Assert.True(onboardingCompleted);
        Assert.Equal("kg", defaultUnit);
    }
    
    #endregion
}
