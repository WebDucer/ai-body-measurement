using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using BodyMeasurement.Models;
using BodyMeasurement.Resources.Strings;
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
    private readonly INavigationService _navigationService;
    private readonly ILogger<MainViewModel> _logger;

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

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private double? _goalWeightKg;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressMarkerBounds))]
    private double _progressValue;

    [ObservableProperty]
    private string _weightLostDisplay = "--";

    [ObservableProperty]
    private string _motivationalMessage = string.Empty;

    [ObservableProperty]
    private string _startWeightDisplay = "--";

    [ObservableProperty]
    private string _goalWeightDisplay = "--";

    [ObservableProperty]
    private string _visceralFatDisplay = "--";

    [ObservableProperty]
    private string _waterDisplay = "--";

    [ObservableProperty]
    private string _muscleDisplay = "--";

    [ObservableProperty]
    private string _bmiDisplay = "--";

    public Rect ProgressMarkerBounds => new Rect(ProgressValue, 0, 4, 40);

    public MainViewModel(
        IDatabaseService databaseService,
        IStatisticsService statisticsService,
        ISettingsService settingsService,
        INavigationService navigationService,
        ILogger<MainViewModel> logger)
    {
        _databaseService = databaseService;
        _statisticsService = statisticsService;
        _settingsService = settingsService;
        _navigationService = navigationService;
        _logger = logger;

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

            var entries = await _databaseService.GetMeasurementHistoryAsync();
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
            _logger.LogError(ex, "Error loading weight entries");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads statistics data and computes home screen display values
    /// </summary>
    private async Task LoadStatisticsAsync()
    {
        CurrentWeight = await _statisticsService.GetCurrentWeightAsync();
        StartingWeight = await _statisticsService.GetStartingWeightAsync();

        var (absolute, percentage) = await _statisticsService.CalculateWeightChangeAsync();
        WeightChangeAbsolute = absolute;
        WeightChangePercentage = percentage;

        // Load user profile from settings
        var name = _settingsService.UserName;
        UserName = string.IsNullOrWhiteSpace(name) ? "LemoWeight" : name;
        GoalWeightKg = _settingsService.GoalWeightKg;

        // Display strings for start/goal
        StartWeightDisplay = StartingWeight.HasValue ? $"{StartingWeight.Value:F1} kg" : "--";
        GoalWeightDisplay = GoalWeightKg.HasValue ? $"{GoalWeightKg.Value:F1} kg" : "--";

        // Progress calculation: clamp((start - current) / (start - goal), 0, 1)
        if (StartingWeight.HasValue && CurrentWeight.HasValue &&
            GoalWeightKg.HasValue && StartingWeight.Value != GoalWeightKg.Value)
        {
            var raw = (StartingWeight.Value - CurrentWeight.Value) /
                      (StartingWeight.Value - GoalWeightKg.Value);
            ProgressValue = Math.Clamp(raw, 0.0, 1.0);
        }
        else
        {
            ProgressValue = 0;
        }

        // Weight lost display and motivational message
        if (StartingWeight.HasValue && CurrentWeight.HasValue)
        {
            var lost = StartingWeight.Value - CurrentWeight.Value;
            WeightLostDisplay = $"{lost:+0.0;-0.0;0} kg";
            MotivationalMessage = lost > 0
                ? string.Format(Strings.WeightLostFormat, lost)
                : Strings.MotivationalKeepGoing;
        }
        else
        {
            WeightLostDisplay = "--";
            MotivationalMessage = Strings.MotivationalKeepGoing;
        }
    }

    /// <summary>
    /// Deletes a weight entry with confirmation
    /// </summary>
    [RelayCommand]
    private async Task DeleteWeightEntryAsync(int entryId)
    {
        try
        {
            var result = await _navigationService.ShowConfirmationAsync(
                Strings.ConfirmDeleteTitle,
                Strings.ConfirmDeleteMessage,
                Strings.Yes,
                Strings.No);

            if (!result)
                return;

            await _databaseService.RemoveMeasurementAsync(entryId);
            await LoadWeightEntriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting weight entry");
            await _navigationService.ShowAlertAsync(
                Strings.ErrorTitle,
                Strings.ErrorDeleteMeasurement,
                Strings.Ok);
        }
    }

    /// <summary>
    /// Navigates to add weight page
    /// </summary>
    [RelayCommand]
    private async Task NavigateToAddWeightAsync()
    {
        await _navigationService.OpenAddMeasurementAsync();
    }

    /// <summary>
    /// Navigates to edit weight page
    /// </summary>
    [RelayCommand]
    private async Task NavigateToEditWeightAsync(int entryId)
    {
        await _navigationService.OpenEditMeasurementAsync(entryId);
    }

    /// <summary>
    /// Navigates to the Settings tab
    /// </summary>
    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await Shell.Current.GoToAsync("//settings");
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
