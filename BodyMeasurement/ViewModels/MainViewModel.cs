using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BodyMeasurement.Models;
using BodyMeasurement.Services;

namespace BodyMeasurement.ViewModels;

/// <summary>
/// ViewModel for the main page displaying weight measurements list
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;
    private readonly IStatisticsService _statisticsService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private ObservableCollection<WeightEntry> _weightEntries = new();

    [ObservableProperty]
    private double? _currentWeight;

    [ObservableProperty]
    private double? _startingWeight;

    [ObservableProperty]
    private double? _weightChangeAbsolute;

    [ObservableProperty]
    private double? _weightChangePercentage;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _preferredUnit = "kg";

    public MainViewModel(
        IDatabaseService databaseService,
        IStatisticsService statisticsService,
        ISettingsService settingsService)
    {
        _databaseService = databaseService;
        _statisticsService = statisticsService;
        _settingsService = settingsService;

        _preferredUnit = _settingsService.PreferredUnit;
    }

    /// <summary>
    /// Loads weight entries from the database
    /// </summary>
    [RelayCommand]
    private async Task LoadWeightEntriesAsync()
    {
        try
        {
            IsLoading = true;

            var entries = await _databaseService.GetAllWeightEntriesAsync();
            WeightEntries.Clear();
            foreach (var entry in entries)
            {
                WeightEntries.Add(entry);
            }

            IsEmpty = WeightEntries.Count == 0;

            // Update statistics
            await LoadStatisticsAsync();
        }
        catch (Exception ex)
        {
            // Log error (in production, use proper logging)
            System.Diagnostics.Debug.WriteLine($"Error loading weight entries: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads statistics data
    /// </summary>
    private async Task LoadStatisticsAsync()
    {
        CurrentWeight = await _statisticsService.GetCurrentWeightAsync();
        StartingWeight = await _statisticsService.GetStartingWeightAsync();

        var (absolute, percentage) = await _statisticsService.CalculateWeightChangeAsync();
        WeightChangeAbsolute = absolute;
        WeightChangePercentage = percentage;
    }

    /// <summary>
    /// Deletes a weight entry with confirmation
    /// </summary>
    [RelayCommand]
    private async Task DeleteWeightEntryAsync(int entryId)
    {
        try
        {
            // In a real app, show confirmation dialog first
            var result = await Application.Current!.MainPage!.DisplayAlert(
                "Confirm Delete",
                "Are you sure you want to delete this measurement?",
                "Yes",
                "No");

            if (!result)
                return;

            await _databaseService.DeleteWeightEntryAsync(entryId);
            await LoadWeightEntriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting weight entry: {ex.Message}");
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                "Failed to delete measurement",
                "OK");
        }
    }

    /// <summary>
    /// Navigates to add weight page
    /// </summary>
    [RelayCommand]
    private async Task NavigateToAddWeightAsync()
    {
        await Shell.Current.GoToAsync("addweight");
    }

    /// <summary>
    /// Navigates to edit weight page
    /// </summary>
    [RelayCommand]
    private async Task NavigateToEditWeightAsync(int entryId)
    {
        await Shell.Current.GoToAsync($"editweight?id={entryId}");
    }

    /// <summary>
    /// Formats weight in the preferred unit
    /// </summary>
    public string FormatWeight(double weightKg)
    {
        return WeightConverter.Format(weightKg, PreferredUnit);
    }

    /// <summary>
    /// Formats weight change with trend indicator
    /// </summary>
    public string FormatWeightChange()
    {
        if (!WeightChangeAbsolute.HasValue)
            return "--";

        var sign = WeightChangeAbsolute.Value >= 0 ? "+" : "";
        var arrow = WeightChangeAbsolute.Value < 0 ? "↓" : WeightChangeAbsolute.Value > 0 ? "↑" : "";
        
        var formatted = WeightConverter.Format(Math.Abs(WeightChangeAbsolute.Value), PreferredUnit);
        var percentage = WeightChangePercentage.HasValue ? $" ({sign}{WeightChangePercentage.Value:F1}%)" : "";
        
        return $"{sign}{formatted}{percentage} {arrow}";
    }
}
