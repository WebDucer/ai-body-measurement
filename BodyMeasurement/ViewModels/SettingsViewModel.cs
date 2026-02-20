using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using BodyMeasurement.Services;

namespace BodyMeasurement.ViewModels;

/// <summary>
/// ViewModel for settings page
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IExportService _exportService;
    private readonly IDatabaseService _databaseService;
    private readonly ILocalizationService _localizationService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<SettingsViewModel> _logger;

    [ObservableProperty]
    private string _selectedLanguage;

    [ObservableProperty]
    private string _selectedUnit;

    [ObservableProperty]
    private bool _isExporting;

    public SettingsViewModel(
        ISettingsService settingsService,
        IExportService exportService,
        IDatabaseService databaseService,
        ILocalizationService localizationService,
        INavigationService navigationService,
        ILogger<SettingsViewModel> logger)
    {
        _settingsService = settingsService;
        _exportService = exportService;
        _databaseService = databaseService;
        _localizationService = localizationService;
        _navigationService = navigationService;
        _logger = logger;

        // Load current settings
        _selectedLanguage = _settingsService.Language;
        _selectedUnit = _settingsService.PreferredUnit;
    }

    /// <summary>
    /// Called when language selection changes
    /// </summary>
    partial void OnSelectedLanguageChanged(string value)
    {
        if (string.IsNullOrEmpty(value) || value == _localizationService.CurrentLanguage)
            return;

        // Update the localization service which will change the culture
        _localizationService.SetLanguage(value);

        // Show message and restart the app to apply changes
        // Application.Current is used directly here intentionally — the Dispatcher and window
        // recreation is app-lifecycle specific and has no sensible interface abstraction.
        Application.Current?.Dispatcher.Dispatch(async () =>
        {
            bool shouldRestart = await Application.Current.Windows[0].Page!.DisplayAlertAsync(
                "Language Changed",
                "The app needs to restart to apply the language change. Restart now?",
                "Yes",
                "No");

            if (shouldRestart)
            {
                // Restart the app by recreating the main window
                var window = Application.Current.Windows.FirstOrDefault();
                if (window != null)
                {
                    window.Page = new AppShell();
                }
            }
        });
    }

    /// <summary>
    /// Called when unit selection changes
    /// </summary>
    partial void OnSelectedUnitChanged(string value)
    {
        _settingsService.PreferredUnit = value;
    }

    /// <summary>
    /// Exports data to CSV
    /// </summary>
    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        try
        {
            IsExporting = true;

            var entries = await _databaseService.GetMeasurementHistoryAsync();

            if (entries.Count == 0)
            {
                await _navigationService.ShowAlertAsync(
                    _localizationService.GetString("NoDataTitle"),
                    _localizationService.GetString("NoDataExportMessage"),
                    "OK");
                return;
            }

            var filePath = await _exportService.ExportToCsvAsync(entries, _settingsService.Language);
            var shared = await _exportService.ShareFileAsync(filePath);

            if (shared)
            {
                await _navigationService.ShowAlertAsync(
                    _localizationService.GetString("ExportSuccessTitle"),
                    _localizationService.GetString("ExportSuccessMessage"),
                    "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting CSV");
            await _navigationService.ShowAlertAsync(
                _localizationService.GetString("ErrorTitle"),
                _localizationService.GetString("ErrorExportData"),
                "OK");
        }
        finally
        {
            IsExporting = false;
        }
    }

    /// <summary>
    /// Exports data to JSON
    /// </summary>
    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        try
        {
            IsExporting = true;

            var entries = await _databaseService.GetMeasurementHistoryAsync();

            if (entries.Count == 0)
            {
                await _navigationService.ShowAlertAsync(
                    _localizationService.GetString("NoDataTitle"),
                    _localizationService.GetString("NoDataExportMessage"),
                    "OK");
                return;
            }

            var filePath = await _exportService.ExportToJsonAsync(entries);
            var shared = await _exportService.ShareFileAsync(filePath);

            if (shared)
            {
                await _navigationService.ShowAlertAsync(
                    _localizationService.GetString("ExportSuccessTitle"),
                    _localizationService.GetString("ExportSuccessMessage"),
                    "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting JSON");
            await _navigationService.ShowAlertAsync(
                _localizationService.GetString("ErrorTitle"),
                _localizationService.GetString("ErrorExportData"),
                "OK");
        }
        finally
        {
            IsExporting = false;
        }
    }
}
