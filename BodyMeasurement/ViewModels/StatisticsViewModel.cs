using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using BodyMeasurement.Models;
using BodyMeasurement.Resources.Strings;
using BodyMeasurement.Services;

namespace BodyMeasurement.ViewModels;

/// <summary>
/// ViewModel for statistics page showing weight progress and trends
/// </summary>
public partial class StatisticsViewModel : ObservableObject
{
    private readonly IStatisticsService _statisticsService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<StatisticsViewModel> _logger;

    [ObservableProperty]
    private Statistics? _statistics;

    [ObservableProperty]
    private string _selectedPeriod;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasData;

    [ObservableProperty]
    private string _preferredUnit = "kg";

    public StatisticsViewModel(
        IStatisticsService statisticsService,
        ISettingsService settingsService,
        ILogger<StatisticsViewModel> logger)
    {
        _statisticsService = statisticsService;
        _settingsService = settingsService;
        _logger = logger;

        _preferredUnit = _settingsService.PreferredUnit;
        _selectedPeriod = Strings.PeriodLabelAllTime;
    }

    /// <summary>
    /// Loads statistics based on selected period
    /// </summary>
    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        try
        {
            IsLoading = true;

            int? periodDays = null;
            if (SelectedPeriod == Strings.PeriodLabel7Days)
                periodDays = 7;
            else if (SelectedPeriod == Strings.PeriodLabel30Days)
                periodDays = 30;
            else if (SelectedPeriod == Strings.PeriodLabel90Days)
                periodDays = 90;
            else if (SelectedPeriod == Strings.PeriodLabelAllTime)
                periodDays = null;

            Statistics = await _statisticsService.GetStatisticsAsync(periodDays);
            HasData = Statistics?.TotalMeasurements > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading statistics");
            HasData = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Changes the selected period and reloads statistics
    /// </summary>
    [RelayCommand]
    private async Task ChangePeriodAsync(string period)
    {
        SelectedPeriod = period;
        await LoadStatisticsAsync();
    }

    /// <summary>
    /// Formats weight in the preferred unit
    /// </summary>
    public string FormatWeight(double? weightKg)
    {
        if (!weightKg.HasValue)
            return "--";

        return WeightConverter.Format(weightKg.Value, PreferredUnit);
    }

    /// <summary>
    /// Formats weight change with sign and arrow
    /// </summary>
    public string FormatWeightChange(double? weightChangeKg, double? percentage)
    {
        if (!weightChangeKg.HasValue)
            return "--";

        var sign = weightChangeKg.Value >= 0 ? "+" : "";
        var arrow = weightChangeKg.Value < 0 ? "↓" : weightChangeKg.Value > 0 ? "↑" : "";
        
        var formatted = WeightConverter.Format(Math.Abs(weightChangeKg.Value), PreferredUnit);
        var percentageStr = percentage.HasValue ? $" ({sign}{percentage.Value:F1}%)" : "";
        
        return $"{sign}{formatted}{percentageStr} {arrow}";
    }

    /// <summary>
    /// Gets trend text (gaining, losing, maintaining)
    /// </summary>
    public string GetTrendText()
    {
        if (Statistics?.WeightChangeKg == null)
            return Strings.NoData;

        if (Statistics.WeightChangeKg < -0.5)
            return Strings.TrendLosing;
        else if (Statistics.WeightChangeKg > 0.5)
            return Strings.TrendGaining;
        else
            return Strings.TrendMaintaining;
    }
}
