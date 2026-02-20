using BodyMeasurement.Extensions;
using BodyMeasurement.Models;
using BodyMeasurement.Services;
using BodyMeasurement.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BodyMeasurement.Tests.Extensions;

/// <summary>
/// Tests for ServiceCollectionExtensions.AddViewWithViewModel (task 11.6)
/// </summary>
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddViewWithViewModel_RegistersBothTypesInContainer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register dependencies needed by AddEditWeightViewModel
        services.AddSingleton(new Mock<IDatabaseService>().Object);
        services.AddSingleton(new Mock<ISettingsService>().Object);
        services.AddSingleton(new Mock<INavigationService>().Object);
        services.AddSingleton(new Mock<ILocalizationService>().Object);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        services.AddLogging();

        // Act - use the extension method
        services.AddViewWithViewModel<AddEditWeightViewModel, AddEditWeightViewModel>(ServiceLifetime.Transient);

        // Assert - both types are registered
        var provider = services.BuildServiceProvider();
        var descriptors = services.Where(d => d.ServiceType == typeof(AddEditWeightViewModel)).ToList();
        Assert.NotEmpty(descriptors);
    }

    [Fact]
    public void AddViewWithViewModel_DefaultLifetimeIsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - call without explicit lifetime
        services.AddViewWithViewModel<AddEditWeightViewModel, AddEditWeightViewModel>();

        // Assert - the registered descriptors should use Transient lifetime
        var descriptors = services.Where(d => d.ServiceType == typeof(AddEditWeightViewModel)).ToList();
        Assert.All(descriptors, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AddViewWithViewModel_WithSingletonLifetime_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewWithViewModel<AddEditWeightViewModel, AddEditWeightViewModel>(ServiceLifetime.Singleton);

        // Assert
        var descriptors = services.Where(d => d.ServiceType == typeof(AddEditWeightViewModel)).ToList();
        Assert.All(descriptors, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AddViewWithViewModel_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddViewWithViewModel<AddEditWeightViewModel, AddEditWeightViewModel>();

        // Assert - fluent API: returns the same instance
        Assert.Same(services, result);
    }

    [Fact]
    public void AddViewWithViewModel_RegistersViewModelServiceType()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewWithViewModel<AddEditWeightViewModel, AddEditWeightViewModel>();

        // Assert - the ViewModel type is registered as its own service type
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(AddEditWeightViewModel));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(AddEditWeightViewModel), descriptor.ImplementationType);
    }
}
