using Microsoft.Extensions.Logging;
using BodyMeasurement.Views;
using BodyMeasurement.ViewModels;
using BodyMeasurement.Services;
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
			});

		// Register Views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<AddEditWeightPage>();
		builder.Services.AddTransient<StatisticsPage>();
		builder.Services.AddTransient<ChartPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<OnboardingPage>();

		// Register Services
		builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
		builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
		builder.Services.AddSingleton<IExportService, ExportService>();

		// Register ViewModels
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<AddEditWeightViewModel>();
		builder.Services.AddTransient<StatisticsViewModel>();
		builder.Services.AddTransient<ChartViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
