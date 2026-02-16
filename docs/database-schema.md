# Database Schema Documentation

## Overview

The Body Measurement App uses **SQLite** as its local database engine with **sqlite-net-pcl** as the ORM. All data is stored locally on the device for offline-first functionality.

## Database Location

The SQLite database file (`bodymeasurement.db3`) is stored in the platform-specific application data directory:

- **iOS**: `~/Library/Application Support/page.eugen.maui.ai.bodymeasurement/bodymeasurement.db3`
- **Android**: `/data/data/page.eugen.maui.ai.bodymeasurement/files/bodymeasurement.db3`
- **Windows**: `%LOCALAPPDATA%\page.eugen.maui.ai.bodymeasurement\bodymeasurement.db3`
- **macOS**: `~/Library/Containers/page.eugen.maui.ai.bodymeasurement/Data/Library/Application Support/bodymeasurement.db3`

## Schema Version 1

### Tables

#### WeightEntries

Primary table for storing weight measurements.

| Column     | Type     | Constraints            | Description                           |
|------------|----------|------------------------|---------------------------------------|
| Id         | INTEGER  | PRIMARY KEY, AUTOINCREMENT | Unique identifier                |
| Date       | DATETIME | NOT NULL               | Date of measurement                   |
| WeightKg   | REAL     | NOT NULL               | Weight in kilograms (normalized)      |
| Notes      | TEXT     | NULL                   | Optional user notes                   |
| CreatedAt  | DATETIME | NOT NULL               | Timestamp when entry was created      |

**Indexes:**
- Index on `Date` (descending) for efficient chronological queries

**Constraints:**
- `WeightKg` must be > 0
- `Date` cannot be in the future

### Model Definition

```csharp
[Table("WeightEntries")]
public class WeightEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public double WeightKg { get; set; }  // Always stored in kg
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
```

## Data Storage Strategy

### Weight Units

- **Internal Storage**: All weights are stored in **kilograms (kg)** regardless of user preference
- **Display**: Weights are converted to the user's preferred unit (kg or lbs) at display time
- **Conversion Formula**: 
  - `kg → lbs`: `weight_kg * 2.20462`
  - `lbs → kg`: `weight_lbs / 2.20462`

**Rationale**: Storing in a single unit (kg) ensures data consistency and simplifies queries/calculations.

### Date Handling

- **Storage**: Dates are stored as `DateTime` in UTC
- **Display**: Dates are converted to local timezone for display
- **Validation**: Future dates are rejected to prevent invalid entries

### Notes Field

- Optional field for user annotations
- Supports multiline text
- Special characters (quotes, commas, newlines) are properly escaped in exports

## Database Operations

### Initialization

Database is initialized on first app launch:

```csharp
public async Task InitializeAsync()
{
    await _database.CreateTableAsync<WeightEntry>();
}
```

### CRUD Operations

#### Insert

```csharp
public async Task<int> InsertWeightEntryAsync(WeightEntry entry)
{
    entry.CreatedAt = DateTime.Now;
    return await _database.InsertAsync(entry);
}
```

#### Read

```csharp
// Get all entries (sorted by date descending)
public async Task<List<WeightEntry>> GetAllWeightEntriesAsync()
{
    return await _database.Table<WeightEntry>()
        .OrderByDescending(e => e.Date)
        .ToListAsync();
}

// Get by ID
public async Task<WeightEntry?> GetWeightEntryByIdAsync(int id)
{
    return await _database.Table<WeightEntry>()
        .Where(e => e.Id == id)
        .FirstOrDefaultAsync();
}

// Get by date range
public async Task<List<WeightEntry>> GetWeightEntriesInDateRangeAsync(
    DateTime startDate, DateTime endDate)
{
    return await _database.Table<WeightEntry>()
        .Where(e => e.Date >= startDate && e.Date <= endDate)
        .OrderByDescending(e => e.Date)
        .ToListAsync();
}
```

#### Update

```csharp
public async Task<int> UpdateWeightEntryAsync(WeightEntry entry)
{
    return await _database.UpdateAsync(entry);
}
```

#### Delete

```csharp
public async Task<int> DeleteWeightEntryAsync(int id)
{
    return await _database.DeleteAsync<WeightEntry>(id);
}
```

## Migration Strategy

### Current Version (V1)

No migrations needed - this is the initial schema.

### Future Migrations

When adding new columns or tables in future versions:

1. **Add Migration Method** in `DatabaseService`:

```csharp
private async Task MigrateToVersion2Async()
{
    // Example: Add new column
    await _database.ExecuteAsync(
        "ALTER TABLE WeightEntries ADD COLUMN BodyFatPercentage REAL");
}
```

2. **Version Tracking** using SQLite `PRAGMA user_version`:

```csharp
private async Task<int> GetDatabaseVersionAsync()
{
    var result = await _database.ExecuteScalarAsync<int>(
        "PRAGMA user_version");
    return result;
}

private async Task SetDatabaseVersionAsync(int version)
{
    await _database.ExecuteAsync($"PRAGMA user_version = {version}");
}
```

3. **Apply Migrations** on initialization:

```csharp
public async Task InitializeAsync()
{
    await _database.CreateTableAsync<WeightEntry>();
    
    int currentVersion = await GetDatabaseVersionAsync();
    
    if (currentVersion < 2)
    {
        await MigrateToVersion2Async();
        await SetDatabaseVersionAsync(2);
    }
    
    // Future migrations...
}
```

### Migration Best Practices

- **Backward Compatibility**: New versions should read old data
- **Non-Destructive**: Never drop columns with user data
- **Testable**: Write unit tests for each migration
- **Documented**: Document schema changes in this file

## Data Backup and Export

### Export Formats

Users can export data in two formats:

#### CSV Export

```csv
Date,Weight (kg),Weight (lbs),Notes
2026-02-16,75.5,166.4,"Morning measurement"
2026-02-15,76.0,167.6,""
```

- Header row included
- Localized based on user language
- Special characters properly escaped

#### JSON Export

```json
{
  "exportDate": "2026-02-16T10:30:00Z",
  "version": "1.0",
  "entries": [
    {
      "date": "2026-02-16T06:00:00Z",
      "weightKg": 75.5,
      "weightLbs": 166.4,
      "notes": "Morning measurement"
    }
  ]
}
```

- Structured format for programmatic access
- Includes both kg and lbs values
- ISO 8601 date format

### Data Import (Future)

Currently not implemented. Future versions may support:
- Import from CSV
- Import from JSON
- Import from other health apps

## Performance Considerations

### Indexing

- **Date Index**: Speeds up chronological queries and date range filters
- **Primary Key**: Automatic index on `Id` column

### Query Optimization

- Use parameterized queries to prevent SQL injection
- Limit result sets when displaying charts (e.g., last 365 days)
- Use `WHERE` clauses for date filtering instead of loading all data

### Database Size

Estimated storage per entry: ~100 bytes
- 365 entries/year ≈ 36 KB
- 10 years of daily measurements ≈ 360 KB

**Conclusion**: Database size is negligible for this use case.

## Data Privacy and Security

### Local-Only Storage

- **No Cloud Sync**: Data never leaves the device
- **No Analytics**: No telemetry or usage tracking
- **No Third-Party Access**: Only the app can access the database

### Platform Security

- **iOS**: App Sandbox and Keychain protection
- **Android**: App-private storage (not accessible without root)
- **Windows**: User-specific AppData folder
- **macOS**: App Sandbox with container isolation

### Data Deletion

When the app is uninstalled:
- **iOS/Android**: Database is automatically deleted
- **Windows/macOS**: User must manually delete app data folder (documented in README)

## Troubleshooting

### Database Corruption

If database becomes corrupted:
1. Export data if possible
2. Delete database file
3. Restart app (new database will be created)
4. Re-import data from export

### Reset Database

For testing or troubleshooting:

```csharp
// Delete database file
File.Delete(DatabasePath);

// Reinitialize
await DatabaseService.InitializeAsync();
```

### Debug Queries

Enable SQLite logging in debug builds:

```csharp
#if DEBUG
SQLite.SQLiteConnection.Trace = true;
#endif
```

## Future Schema Enhancements (V2+)

Potential additions for future versions:

### Additional Body Measurements

```sql
CREATE TABLE BodyMeasurements (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date DATETIME NOT NULL,
    WeightKg REAL,
    BodyFatPercentage REAL,
    WaistCircumferenceCm REAL,
    Notes TEXT,
    CreatedAt DATETIME NOT NULL
);
```

### Goals and Targets

```sql
CREATE TABLE Goals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TargetWeightKg REAL NOT NULL,
    TargetDate DATETIME,
    IsActive BOOLEAN NOT NULL,
    CreatedAt DATETIME NOT NULL
);
```

### Cloud Sync Metadata

```sql
CREATE TABLE SyncMetadata (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EntityType TEXT NOT NULL,
    EntityId INTEGER NOT NULL,
    LastSyncedAt DATETIME,
    CloudId TEXT
);
```

## References

- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [sqlite-net-pcl GitHub](https://github.com/praeclarum/sqlite-net)
- [MAUI Data Storage](https://learn.microsoft.com/dotnet/maui/data-cloud/database-sqlite)
