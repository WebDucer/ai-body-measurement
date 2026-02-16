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
    /// Inserts a new weight entry into the database
    /// </summary>
    public async Task<int> InsertWeightEntryAsync(WeightEntry entry)
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
            throw new InvalidOperationException("Failed to insert weight entry", ex);
        }
    }

    /// <summary>
    /// Gets all weight entries sorted by date descending (most recent first)
    /// </summary>
    public async Task<List<WeightEntry>> GetAllWeightEntriesAsync()
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
            throw new InvalidOperationException("Failed to retrieve weight entries", ex);
        }
    }

    /// <summary>
    /// Gets a weight entry by its ID
    /// </summary>
    public async Task<WeightEntry?> GetWeightEntryByIdAsync(int id)
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
            throw new InvalidOperationException($"Failed to retrieve weight entry with ID {id}", ex);
        }
    }

    /// <summary>
    /// Updates an existing weight entry
    /// </summary>
    public async Task<int> UpdateWeightEntryAsync(WeightEntry entry)
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.UpdateAsync(entry);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update weight entry with ID {entry.Id}", ex);
        }
    }

    /// <summary>
    /// Deletes a weight entry
    /// </summary>
    public async Task<int> DeleteWeightEntryAsync(int id)
    {
        await EnsureInitializedAsync();

        try
        {
            return await _database!.DeleteAsync<WeightEntry>(id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete weight entry with ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets weight entries within a date range
    /// </summary>
    public async Task<List<WeightEntry>> GetWeightEntriesInDateRangeAsync(DateTime startDate, DateTime endDate)
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
            throw new InvalidOperationException($"Failed to retrieve weight entries between {startDate:d} and {endDate:d}", ex);
        }
    }
}
