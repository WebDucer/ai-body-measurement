using BodyMeasurement.Models;
using BodyMeasurement.Services;
using BodyMeasurement.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;

namespace BodyMeasurement.Tests.ViewModels;

/// <summary>
/// Tests for AddEditWeightViewModel.ApplyQueryAttributes (task 11.5)
/// </summary>
public class ApplyQueryAttributesTests
{
    private readonly Mock<IDatabaseService> _mockDb;
    private readonly Mock<ISettingsService> _mockSettings;
    private readonly Mock<INavigationService> _mockNav;
    private readonly Mock<ILogger<AddEditWeightViewModel>> _mockLogger;

    public ApplyQueryAttributesTests()
    {
        _mockDb = new Mock<IDatabaseService>();
        _mockSettings = new Mock<ISettingsService>();
        _mockNav = new Mock<INavigationService>();
        _mockLogger = new Mock<ILogger<AddEditWeightViewModel>>();

        _mockSettings.Setup(s => s.PreferredUnit).Returns("kg");
    }

    private AddEditWeightViewModel CreateViewModel() =>
        new AddEditWeightViewModel(
            _mockDb.Object,
            _mockSettings.Object,
            _mockNav.Object,
            _mockLogger.Object);

    [Fact]
    public void ApplyQueryAttributes_WithMeasurementId_SetsEditMode()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var query = new Dictionary<string, object> { { "MeasurementId", 42 } };

        _mockDb.Setup(x => x.FindMeasurementAsync(42))
               .ReturnsAsync(new WeightEntry { Id = 42, WeightKg = 75.0, Date = DateTime.Today });

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.True(viewModel.IsEditMode);
    }

    [Fact]
    public void ApplyQueryAttributes_WithMeasurementId_SetsTitleToEditWeight()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var query = new Dictionary<string, object> { { "MeasurementId", 5 } };

        _mockDb.Setup(x => x.FindMeasurementAsync(5))
               .ReturnsAsync(new WeightEntry { Id = 5, WeightKg = 70.0, Date = DateTime.Today });

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Equal("Edit Weight", viewModel.Title);
    }

    [Fact]
    public void ApplyQueryAttributes_WithMeasurementId_CallsLoadWeightEntry()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var query = new Dictionary<string, object> { { "MeasurementId", 7 } };

        _mockDb.Setup(x => x.FindMeasurementAsync(7))
               .ReturnsAsync(new WeightEntry { Id = 7, WeightKg = 80.0, Date = DateTime.Today });

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Allow the fire-and-forget task to complete
        Thread.Sleep(50);

        // Assert
        _mockDb.Verify(x => x.FindMeasurementAsync(7), Times.Once);
    }

    [Fact]
    public void ApplyQueryAttributes_WithoutMeasurementId_SetsAddMode()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.False(viewModel.IsEditMode);
    }

    [Fact]
    public void ApplyQueryAttributes_WithoutMeasurementId_SetsTitleToAddWeight()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Equal("Add Weight", viewModel.Title);
    }

    [Fact]
    public void ApplyQueryAttributes_WithoutMeasurementId_ResetsForm()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Put the VM into edit mode first
        var editQuery = new Dictionary<string, object> { { "MeasurementId", 1 } };
        _mockDb.Setup(x => x.FindMeasurementAsync(1))
               .ReturnsAsync(new WeightEntry { Id = 1, WeightKg = 75.0, Date = DateTime.Today });
        viewModel.ApplyQueryAttributes(editQuery);

        // Now switch to add mode
        var addQuery = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(addQuery);

        // Assert
        Assert.False(viewModel.IsEditMode);
        Assert.Equal(0, viewModel.Weight);
        Assert.Equal(DateTime.Today, viewModel.Date);
        Assert.Null(viewModel.Notes);
    }

    [Fact]
    public void ApplyQueryAttributes_WithWrongValueType_TreatsAsAddMode()
    {
        // Arrange
        var viewModel = CreateViewModel();
        // Pass a string instead of int for MeasurementId — should not trigger edit mode
        var query = new Dictionary<string, object> { { "MeasurementId", "not-an-int" } };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert — pattern matching `value is int id` fails → add mode
        Assert.False(viewModel.IsEditMode);
    }
}
