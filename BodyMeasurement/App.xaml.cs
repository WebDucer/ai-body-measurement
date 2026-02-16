using Microsoft.Extensions.DependencyInjection;
using BodyMeasurement.Services;
using BodyMeasurement.Views;

namespace BodyMeasurement;

public partial class App : Application
{
	private readonly IServiceProvider _serviceProvider;

	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_serviceProvider = serviceProvider;

		// Initialize localization
		var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
		// The service constructor already sets the language based on preferences

		// Initialize database on app startup
		var databaseService = serviceProvider.GetRequiredService<IDatabaseService>();
		_ = databaseService.InitializeAsync();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();

		// Show onboarding on first launch
		if (!settingsService.OnboardingCompleted)
		{
			var onboardingPage = _serviceProvider.GetRequiredService<OnboardingPage>();
			return new Window(new NavigationPage(onboardingPage));
		}

		return new Window(new AppShell());
	}
}