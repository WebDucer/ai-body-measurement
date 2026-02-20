using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace BodyMeasurement.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a View and its ViewModel together with the same lifetime.
    /// </summary>
    public static IServiceCollection AddViewWithViewModel<TView, TViewModel>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TView : Page
        where TViewModel : ObservableObject
    {
        services.Add(new ServiceDescriptor(typeof(TView), typeof(TView), lifetime));
        services.Add(new ServiceDescriptor(typeof(TViewModel), typeof(TViewModel), lifetime));
        return services;
    }
}
