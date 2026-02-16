using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        ILocalizationService localizationService)
    {
        _settingsService = settingsService;
        _exportService = exportService;
        _databaseService = databaseService;
        _localizationService = localizationService;

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
        Application.Current?.Dispatcher.Dispatch(async () =>
        {
            bool shouldRestart = await Application.Current.MainPage!.DisplayAlert(
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
                    // Close current window and create new one
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
        // In a real app, this would trigger UI refresh with new units
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

            var entries = await _databaseService.GetAllWeightEntriesAsync();
            
            if (entries.Count == 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "No Data",
                    "No measurements to export",
                    "OK");
                return;
            }

            var filePath = await _exportService.ExportToCsvAsync(entries, _settingsService.Language);
            var shared = await _exportService.ShareFileAsync(filePath);

            if (shared)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Data exported successfully",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exporting CSV: {ex.Message}");
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                "Failed to export data",
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

            var entries = await _databaseService.GetAllWeightEntriesAsync();
            
            if (entries.Count == 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "No Data",
                    "No measurements to export",
                    "OK");
                return;
            }

            var filePath = await _exportService.ExportToJsonAsync(entries);
            var shared = await _exportService.ShareFileAsync(filePath);

            if (shared)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Data exported successfully",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exporting JSON: {ex.Message}");
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                "Failed to export data",
                "OK");
        }
        finally
        {
            IsExporting = false;
        }
    }
}
