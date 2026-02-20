# Specification: Local Storage

## ADDED Requirements

### Requirement: App stores all data locally using SQLite
The system SHALL persist all weight measurements in a local SQLite database file on the device.

#### Scenario: Database created on first launch
- **WHEN** user opens app for first time
- **THEN** system creates SQLite database file in app's local storage directory

#### Scenario: Data stored in SQLite
- **WHEN** user records a weight measurement
- **THEN** system writes data to SQLite database table

#### Scenario: Database location
- **WHEN** app stores data
- **THEN** SQLite file is stored in platform-specific app data directory (not accessible to other apps)

### Requirement: Database schema supports weight measurements
The system SHALL use a database schema with a table for weight entries. The `IDatabaseService` interface SHALL expose domain-specific method names instead of generic CRUD names.

#### Scenario: WeightEntries table structure
- **WHEN** database is created
- **THEN** WeightEntries table exists with columns: Id (integer, primary key, auto-increment), Date (datetime), WeightKg (double), Notes (text), CreatedAt (datetime)

#### Scenario: Primary key auto-increments
- **WHEN** inserting new weight entry via `RecordMeasurementAsync`
- **THEN** Id is automatically assigned as next sequential integer

#### Scenario: Date column stores full datetime
- **WHEN** storing measurement date
- **THEN** database preserves date and time with timezone information

#### Scenario: WeightKg stores decimal precision
- **WHEN** storing weight value
- **THEN** database stores as double-precision floating point (at least 1 decimal place)

#### Scenario: Notes column allows null
- **WHEN** measurement has no notes
- **THEN** database accepts and stores null value in Notes column

### Requirement: IDatabaseService uses domain-specific method names
The system SHALL name database operations after domain concepts rather than generic CRUD verbs, so call sites read as business operations.

#### Scenario: Record new measurement
- **WHEN** a new measurement is saved
- **THEN** `IDatabaseService.RecordMeasurementAsync(WeightEntry)` is called and returns the number of rows inserted

#### Scenario: Retrieve full measurement history
- **WHEN** the measurement list is loaded
- **THEN** `IDatabaseService.GetMeasurementHistoryAsync()` returns all entries sorted by date descending

#### Scenario: Find single measurement by ID
- **WHEN** a specific measurement is needed for editing
- **THEN** `IDatabaseService.FindMeasurementAsync(int id)` returns the matching entry or `null` if not found

#### Scenario: Update existing measurement
- **WHEN** an edited measurement is saved
- **THEN** `IDatabaseService.UpdateMeasurementAsync(WeightEntry)` updates the record and returns the number of rows affected

#### Scenario: Remove measurement
- **WHEN** a measurement is deleted
- **THEN** `IDatabaseService.RemoveMeasurementAsync(int id)` deletes the record and returns the number of rows affected (0 if not found)

#### Scenario: Retrieve measurements within a period
- **WHEN** chart or statistics need data for a date range
- **THEN** `IDatabaseService.GetMeasurementsInPeriodAsync(DateTime start, DateTime end)` returns matching entries sorted by date descending

### Requirement: App settings stored using platform preferences
The system SHALL store user preferences (language, unit, onboarding status) using .NET MAUI Preferences API, not database.

#### Scenario: Preferences stored in platform storage
- **WHEN** user changes preferred unit
- **THEN** system stores setting using platform-native preferences (iOS UserDefaults, Android SharedPreferences, Windows ApplicationData)

#### Scenario: Preferences persist across app launches
- **WHEN** user sets preferred unit to lbs, closes app, and reopens
- **THEN** preferred unit remains lbs

#### Scenario: Preferences separate from measurement data
- **WHEN** querying database
- **THEN** user preferences are not stored in SQLite tables

### Requirement: Data access is thread-safe
The system SHALL ensure database operations are thread-safe to prevent data corruption.

#### Scenario: Concurrent read operations
- **WHEN** multiple components read from database simultaneously
- **THEN** all operations complete successfully without errors

#### Scenario: Write operations are serialized
- **WHEN** attempting simultaneous write operations
- **THEN** database handles operations sequentially without corruption

### Requirement: Database operations are asynchronous
The system SHALL perform database operations asynchronously to avoid blocking UI thread.

#### Scenario: Async read operation
- **WHEN** loading weight measurements
- **THEN** UI remains responsive during database query

#### Scenario: Async write operation
- **WHEN** saving new measurement
- **THEN** UI remains responsive during database insert

#### Scenario: Async delete operation
- **WHEN** deleting measurement
- **THEN** UI remains responsive during database delete

### Requirement: Database handles large datasets efficiently
The system SHALL maintain performance even with large numbers of measurements.

#### Scenario: Query 1000+ measurements
- **WHEN** loading measurements with 1000+ entries
- **THEN** query completes within 2 seconds

#### Scenario: Insert operation speed
- **WHEN** inserting new measurement
- **THEN** operation completes within 500 milliseconds

#### Scenario: Delete operation speed
- **WHEN** deleting measurement
- **THEN** operation completes within 500 milliseconds

### Requirement: Database transactions ensure data integrity
The system SHALL use database transactions for operations that modify multiple records or require atomicity.

#### Scenario: Transaction rollback on error
- **WHEN** multi-step database operation fails partway through
- **THEN** all changes are rolled back to maintain consistency

#### Scenario: Successful transaction commits
- **WHEN** multi-step database operation succeeds
- **THEN** all changes are committed atomically

### Requirement: Database schema can evolve
The system SHALL support database schema migrations for future versions.

#### Scenario: Schema version tracked
- **WHEN** database is created
- **THEN** schema version is recorded for future migration support

#### Scenario: Future migration capability
- **WHEN** app is updated with new schema in future
- **THEN** system can detect old schema version and migrate data

### Requirement: Data persists across app updates
The system SHALL preserve user data when app is updated to new version.

#### Scenario: Data survives app update
- **WHEN** user updates app to new version
- **THEN** all previously recorded measurements remain accessible

#### Scenario: Database file location consistent
- **WHEN** app updates
- **THEN** database file remains in same location with same data

### Requirement: App functions fully offline
The system SHALL provide full functionality without requiring network connectivity.

#### Scenario: Record measurement offline
- **WHEN** device has no network connection
- **THEN** user can record measurements normally

#### Scenario: View data offline
- **WHEN** device has no network connection
- **THEN** user can view all recorded data and statistics

#### Scenario: Export data offline
- **WHEN** device has no network connection
- **THEN** user can export data to CSV or JSON

### Requirement: Database errors handled gracefully
The system SHALL handle database errors without crashing or losing data.

#### Scenario: Disk full error
- **WHEN** device storage is full and write operation fails
- **THEN** system displays error message and does not corrupt existing data

#### Scenario: Database locked error
- **WHEN** database is temporarily locked by another operation
- **THEN** system retries or queues operation without crashing

#### Scenario: Corrupted database detection
- **WHEN** database file is corrupted
- **THEN** system detects corruption and attempts recovery or displays error message

### Requirement: Database queries are optimized
The system SHALL use appropriate database indexes and query optimization for performance.

#### Scenario: Date-based queries use index
- **WHEN** querying measurements by date range
- **THEN** query uses date column index for fast retrieval

#### Scenario: Sorting by date optimized
- **WHEN** loading measurements sorted by date
- **THEN** query completes efficiently using indexed column

### Requirement: Data privacy and security
The system SHALL ensure weight data is private and secure on the device.

#### Scenario: Data not accessible by other apps
- **WHEN** data is stored
- **THEN** database file is protected by platform app sandbox (other apps cannot access)

#### Scenario: No cloud transmission
- **WHEN** user records or views data
- **THEN** no data is transmitted over network

#### Scenario: Data deleted on app uninstall
- **WHEN** user uninstalls app
- **THEN** all local data (database and preferences) is removed from device per platform standards
