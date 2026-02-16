using BodyMeasurement.Models;

namespace BodyMeasurement.Services;

/// <summary>
/// Implementation of statistics service for weight calculations
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly IDatabaseService _databaseService;

    public StatisticsService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Gets the current weight (most recent measurement)
    /// </summary>
    public async Task<double?> GetCurrentWeightAsync()
    {
        var entries = await _databaseService.GetAllWeightEntriesAsync();
        return entries.FirstOrDefault()?.WeightKg;
    }

    /// <summary>
    /// Gets the starting weight (earliest measurement)
    /// </summary>
    public async Task<double?> GetStartingWeightAsync()
    {
        var entries = await _databaseService.GetAllWeightEntriesAsync();
        return entries.LastOrDefault()?.WeightKg;
    }

    /// <summary>
    /// Calculates overall weight change (absolute and percentage)
    /// </summary>
    public async Task<(double? absolute, double? percentage)> CalculateWeightChangeAsync()
    {
        var currentWeight = await GetCurrentWeightAsync();
        var startingWeight = await GetStartingWeightAsync();

        if (!currentWeight.HasValue || !startingWeight.HasValue || startingWeight.Value == 0)
        {
            return (null, null);
        }

        var absolute = currentWeight.Value - startingWeight.Value;
        var percentage = (absolute / startingWeight.Value) * 100;

        return (absolute, percentage);
    }

    /// <summary>
    /// Calculates weight change for a specific period
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    public async Task<(double? absolute, double? percentage)> CalculateWeightChangeForPeriodAsync(int days)
    {
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-days);

        var entries = await _databaseService.GetWeightEntriesInDateRangeAsync(startDate, endDate);

        if (entries.Count == 0)
        {
            return (null, null);
        }

        // Get the most recent and oldest entry in the period
        var currentEntry = entries.OrderByDescending(e => e.Date).FirstOrDefault();
        var startEntry = entries.OrderBy(e => e.Date).FirstOrDefault();

        if (currentEntry == null || startEntry == null || startEntry.WeightKg == 0)
        {
            return (null, null);
        }

        var absolute = currentEntry.WeightKg - startEntry.WeightKg;
        var percentage = (absolute / startEntry.WeightKg) * 100;

        return (absolute, percentage);
    }

    /// <summary>
    /// Gets comprehensive statistics
    /// </summary>
    public async Task<Statistics> GetStatisticsAsync(int? periodDays = null)
    {
        var statistics = new Statistics();

        List<WeightEntry> entries;
        if (periodDays.HasValue)
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-periodDays.Value);
            entries = await _databaseService.GetWeightEntriesInDateRangeAsync(startDate, endDate);
        }
        else
        {
            entries = await _databaseService.GetAllWeightEntriesAsync();
        }

        if (entries.Count == 0)
        {
            return statistics;
        }

        // Sort by date
        entries = entries.OrderBy(e => e.Date).ToList();

        // Basic stats
        statistics.TotalMeasurements = entries.Count;
        statistics.FirstMeasurementDate = entries.First().Date;
        statistics.LastMeasurementDate = entries.Last().Date;

        // Current and starting weights
        statistics.CurrentWeightKg = entries.Last().WeightKg;
        statistics.StartingWeightKg = entries.First().WeightKg;

        // Weight change
        if (statistics.StartingWeightKg > 0)
        {
            statistics.WeightChangeKg = statistics.CurrentWeightKg - statistics.StartingWeightKg;
            statistics.WeightChangePercentage = (statistics.WeightChangeKg / statistics.StartingWeightKg) * 100;
        }

        return statistics;
    }
}
