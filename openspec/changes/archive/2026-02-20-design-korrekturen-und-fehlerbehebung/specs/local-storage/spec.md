## MODIFIED Requirements

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

## ADDED Requirements

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
