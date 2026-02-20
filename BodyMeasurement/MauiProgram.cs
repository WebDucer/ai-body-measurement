using Microsoft.Extensions.Logging;
using BodyMeasurement.Views;
using BodyMeasurement.ViewModels;
using BodyMeasurement.Services;
using BodyMeasurement.Extensions;
using Syncfusion.Maui.Toolkit.Hosting;

namespace BodyMeasurement;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureSyncfusionToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
			});

		// Views + ViewModels (paired via extension)
		builder.Services
			.AddViewWithViewModel<MainPage, MainViewModel>()
			.AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>()
			.AddViewWithViewModel<StatisticsPage, StatisticsViewModel>()
			.AddViewWithViewModel<ChartPage, ChartViewModel>()
			.AddViewWithViewModel<SettingsPage, SettingsViewModel>();
		builder.Services.AddTransient<OnboardingPage>(); // no ViewModel

		// Services (all Singleton)
		builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
		builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
		builder.Services.AddSingleton<IExportService, ExportService>();
		builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
		builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
