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

    [ObservableProperty]
    private string _selectedLanguage;

    [ObservableProperty]
    private string _selectedUnit;

    [ObservableProperty]
    private bool _isExporting;

    public SettingsViewModel(
        ISettingsService settingsService,
        IExportService exportService,
        IDatabaseService databaseService)
    {
        _settingsService = settingsService;
        _exportService = exportService;
        _databaseService = databaseService;

        // Load current settings
        _selectedLanguage = _settingsService.Language;
        _selectedUnit = _settingsService.PreferredUnit;
    }

    /// <summary>
    /// Called when language selection changes
    /// </summary>
    partial void OnSelectedLanguageChanged(string value)
    {
        _settingsService.Language = value;
        // In a real app, this would trigger UI refresh with new language
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
