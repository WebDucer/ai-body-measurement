using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using BodyMeasurement.Models;
using BodyMeasurement.Services;

namespace BodyMeasurement.ViewModels;

/// <summary>
/// ViewModel for chart page displaying weight trend over time
/// </summary>
public partial class ChartViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<ChartViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<WeightEntry> _chartData = new();

    [ObservableProperty]
    private string _selectedFilter = "1 Month";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasData;

    [ObservableProperty]
    private string _preferredUnit = "kg";

    [ObservableProperty]
    private double? _minWeight;

    [ObservableProperty]
    private double? _maxWeight;

    [ObservableProperty]
    private double? _averageWeight;

    public ChartViewModel(
        IDatabaseService databaseService,
        ISettingsService settingsService,
        ILogger<ChartViewModel> logger)
    {
        _databaseService = databaseService;
        _settingsService = settingsService;
        _logger = logger;

        _preferredUnit = _settingsService.PreferredUnit;
    }

    /// <summary>
    /// Loads chart data based on selected filter
    /// </summary>
    [RelayCommand]
    private async Task LoadChartDataAsync()
    {
        try
        {
            IsLoading = true;
            
            // Clear chart data first to prevent rendering issues
            HasData = false;
            ChartData.Clear();
            
            // Small delay to ensure chart stops rendering
            await Task.Delay(50);

            var endDate = DateTime.Today;
            var startDate = SelectedFilter switch
            {
                "1 Week" => endDate.AddDays(-7),
                "1 Month" => endDate.AddDays(-30),
                "3 Months" => endDate.AddDays(-90),
                "6 Months" => endDate.AddDays(-180),
                "All" => DateTime.MinValue,
                _ => endDate.AddDays(-30)
            };

            List<WeightEntry> entries;
            if (SelectedFilter == "All")
            {
                entries = await _databaseService.GetMeasurementHistoryAsync();
            }
            else
            {
                entries = await _databaseService.GetMeasurementsInPeriodAsync(startDate, endDate);
            }

            // Sort by date ascending for chart display
            entries = entries.OrderBy(e => e.Date).ToList();

            // Only proceed if we have data
            if (entries.Count > 0)
            {
                // Calculate stats first
                MinWeight = entries.Min(e => e.WeightKg);
                MaxWeight = entries.Max(e => e.WeightKg);
                AverageWeight = entries.Average(e => e.WeightKg);
                
                // Add all entries at once using a new collection to avoid multiple updates
                var newData = new ObservableCollection<WeightEntry>(entries);
                ChartData = newData;
                
                // Small delay before showing chart
                await Task.Delay(50);
                HasData = true;
            }
            else
            {
                MinWeight = null;
                MaxWeight = null;
                AverageWeight = null;
                HasData = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading chart data");
            HasData = false;
            ChartData.Clear();
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Changes the selected filter and reloads data
    /// </summary>
    [RelayCommand]
    private async Task ChangeFilterAsync(string filter)
    {
        SelectedFilter = filter;
        await LoadChartDataAsync();
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
    /// Gets the weight range text
    /// </summary>
    public string GetWeightRangeText()
    {
        if (!MinWeight.HasValue || !MaxWeight.HasValue)
            return "--";

        var min = FormatWeight(MinWeight.Value);
        var max = FormatWeight(MaxWeight.Value);
        
        return $"{min} - {max}";
    }
}
