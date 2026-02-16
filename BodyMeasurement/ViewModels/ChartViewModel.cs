using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        ISettingsService settingsService)
    {
        _databaseService = databaseService;
        _settingsService = settingsService;

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
                entries = await _databaseService.GetAllWeightEntriesAsync();
            }
            else
            {
                entries = await _databaseService.GetWeightEntriesInDateRangeAsync(startDate, endDate);
            }

            // Sort by date ascending for chart display
            entries = entries.OrderBy(e => e.Date).ToList();

            ChartData.Clear();
            foreach (var entry in entries)
            {
                ChartData.Add(entry);
            }

            HasData = ChartData.Count > 0;

            // Calculate stats
            if (HasData)
            {
                MinWeight = ChartData.Min(e => e.WeightKg);
                MaxWeight = ChartData.Max(e => e.WeightKg);
                AverageWeight = ChartData.Average(e => e.WeightKg);
            }
            else
            {
                MinWeight = null;
                MaxWeight = null;
                AverageWeight = null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading chart data: {ex.Message}");
            HasData = false;
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
