using SQLite;
using BodyMeasurement.Models;

namespace BodyMeasurement.Services;

/// <summary>
/// Implementation of database service using SQLite
/// </summary>
public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _databasePath;

    public DatabaseService() : this(null)
    {
    }

    public DatabaseService(string? databasePath)
    {
        // Get platform-specific database path
        if (string.IsNullOrEmpty(databasePath))
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _databasePath = Path.Combine(folder, "bodymeasurement.db");
        }
        else
        {
            _databasePath = databasePath;
        }
    }

    /// <summary>
    /// Initializes the database and creates tables if they don't exist
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_database != null)
            return;

        try
        {
            _database = new SQLiteAsyncConnection(_databasePath);
            await _database.CreateTableAsync<WeightEntry>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize database", ex);
        }
    }

    /// <summary>
    /// Ensures database is initialized before operations
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_database == null)
        {
            await InitializeAsync();
        }
    }

    /// <summary>
    /// Records a new measurement in the database
    /// </summary>
    public async Task<int> RecordMeasurementAsync(WeightEntry entry)
    {
        await EnsureInitializedAsync();

        try
        {
            // Set CreatedAt if not already set
            if (entry.CreatedAt == default)
            {
                entry.CreatedAt = DateTime.UtcNow;
            }

            return await _database!.InsertAsync(entry);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to record measurement", ex);
        }
    }

    /// <summary>
    /// Gets the full measurement history sorted by date descending (most recent first)
    /// </summary>
    public async Task<List<WeightEntry>> GetMeasurementHistoryAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.Table<WeightEntry>()
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve measurement history", ex);
        }
    }

    /// <summary>
    /// Finds a measurement by its ID
    /// </summary>
    public async Task<WeightEntry?> FindMeasurementAsync(int id)
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.Table<WeightEntry>()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to find measurement with ID {id}", ex);
        }
    }

    /// <summary>
    /// Updates an existing measurement
    /// </summary>
    public async Task<int> UpdateMeasurementAsync(WeightEntry entry)
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.UpdateAsync(entry);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update measurement with ID {entry.Id}", ex);
        }
    }

    /// <summary>
    /// Removes a measurement
    /// </summary>
    public async Task<int> RemoveMeasurementAsync(int id)
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.DeleteAsync<WeightEntry>(id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to remove measurement with ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets measurements within a date range
    /// </summary>
    public async Task<List<WeightEntry>> GetMeasurementsInPeriodAsync(DateTime startDate, DateTime endDate)
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.Table<WeightEntry>()
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to retrieve measurements between {startDate:d} and {endDate:d}", ex);
        }
    }
}
