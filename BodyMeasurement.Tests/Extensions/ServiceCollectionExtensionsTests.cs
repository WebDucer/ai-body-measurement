using BodyMeasurement.Extensions;
using BodyMeasurement.Views;
using BodyMeasurement.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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

        // Act - use the extension method with a real View (Page subclass) and ViewModel
        services.AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>(ServiceLifetime.Transient);

        // Assert - both types are registered as service descriptors
        var viewDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(AddEditWeightPage));
        var viewModelDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(AddEditWeightViewModel));
        Assert.NotNull(viewDescriptor);
        Assert.NotNull(viewModelDescriptor);
    }

    [Fact]
    public void AddViewWithViewModel_DefaultLifetimeIsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - call without explicit lifetime
        services.AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>();

        // Assert - both registered descriptors use Transient lifetime
        var viewDescriptor = services.First(d => d.ServiceType == typeof(AddEditWeightPage));
        var viewModelDescriptor = services.First(d => d.ServiceType == typeof(AddEditWeightViewModel));
        Assert.Equal(ServiceLifetime.Transient, viewDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Transient, viewModelDescriptor.Lifetime);
    }

    [Fact]
    public void AddViewWithViewModel_WithSingletonLifetime_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>(ServiceLifetime.Singleton);

        // Assert
        var viewDescriptor = services.First(d => d.ServiceType == typeof(AddEditWeightPage));
        var viewModelDescriptor = services.First(d => d.ServiceType == typeof(AddEditWeightViewModel));
        Assert.Equal(ServiceLifetime.Singleton, viewDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, viewModelDescriptor.Lifetime);
    }

    [Fact]
    public void AddViewWithViewModel_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>();

        // Assert - fluent API: returns the same instance
        Assert.Same(services, result);
    }

    [Fact]
    public void AddViewWithViewModel_RegistersViewModelServiceType()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>();

        // Assert - the ViewModel type is registered as its own service type with matching implementation type
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(AddEditWeightViewModel));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(AddEditWeightViewModel), descriptor.ImplementationType);
    }
}
