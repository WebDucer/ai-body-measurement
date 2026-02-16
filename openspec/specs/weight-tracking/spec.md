# Specification: Weight Tracking

## ADDED Requirements

### Requirement: User can record weight measurement
The system SHALL allow users to record a weight measurement with a weight value, date/time, and optional notes.

#### Scenario: Record weight with current date
- **WHEN** user enters weight value and submits without specifying a date
- **THEN** system records the measurement with current date and time

#### Scenario: Record weight with custom date
- **WHEN** user enters weight value, selects a past date, and submits
- **THEN** system records the measurement with the specified date and time

#### Scenario: Record weight with notes
- **WHEN** user enters weight value, adds notes text, and submits
- **THEN** system records the measurement with the weight, date, and notes

#### Scenario: Invalid weight value
- **WHEN** user enters a weight value less than or equal to 0
- **THEN** system displays validation error and prevents submission

#### Scenario: Future date validation
- **WHEN** user selects a date in the future
- **THEN** system displays validation error and prevents submission

### Requirement: User can view all weight measurements
The system SHALL display a list of all recorded weight measurements in reverse chronological order (most recent first).

#### Scenario: View weight measurements list
- **WHEN** user navigates to the weight history view
- **THEN** system displays all measurements sorted by date (newest first)

#### Scenario: Empty measurements list
- **WHEN** user has no recorded measurements
- **THEN** system displays an empty state message prompting to add first measurement

#### Scenario: Measurement display format
- **WHEN** user views a measurement in the list
- **THEN** system displays weight in user's preferred unit (kg or lbs), date, and notes if present

### Requirement: User can edit existing weight measurement
The system SHALL allow users to modify the weight value, date, or notes of an existing measurement.

#### Scenario: Edit weight value
- **WHEN** user selects a measurement, changes the weight value, and saves
- **THEN** system updates the measurement with the new weight value

#### Scenario: Edit measurement date
- **WHEN** user selects a measurement, changes the date, and saves
- **THEN** system updates the measurement with the new date and re-sorts the list

#### Scenario: Edit measurement notes
- **WHEN** user selects a measurement, modifies the notes, and saves
- **THEN** system updates the measurement with the new notes

#### Scenario: Cancel edit operation
- **WHEN** user selects a measurement, makes changes, and cancels
- **THEN** system discards changes and retains original measurement data

### Requirement: User can delete weight measurement
The system SHALL allow users to delete an existing weight measurement.

#### Scenario: Delete measurement with confirmation
- **WHEN** user selects delete action on a measurement and confirms
- **THEN** system permanently removes the measurement from storage

#### Scenario: Cancel delete operation
- **WHEN** user selects delete action on a measurement and cancels
- **THEN** system retains the measurement without changes

### Requirement: Weight measurements persist locally
The system SHALL store all weight measurements in local SQLite database to enable offline-first functionality.

#### Scenario: Data persists across app restarts
- **WHEN** user records measurements, closes app, and reopens
- **THEN** system displays all previously recorded measurements

#### Scenario: Data stored locally only
- **WHEN** user records a measurement
- **THEN** system stores data in local SQLite database without network communication

### Requirement: System supports multiple weight units
The system SHALL support weight entry and display in both kilograms (kg) and pounds (lbs) with automatic conversion.

#### Scenario: Display weight in preferred unit
- **WHEN** user has set preferred unit to kg
- **THEN** system displays all weights in kilograms with appropriate precision

#### Scenario: Switch units
- **WHEN** user changes preferred unit from kg to lbs
- **THEN** system converts and displays all weights in pounds

#### Scenario: Unit conversion accuracy
- **WHEN** system converts weight between kg and lbs
- **THEN** system uses conversion factor 1 kg = 2.20462 lbs with precision to 1 decimal place

### Requirement: Weight data stored in normalized format
The system SHALL store all weight measurements in kilograms internally, regardless of user's preferred display unit.

#### Scenario: Store in kg regardless of input unit
- **WHEN** user enters weight in lbs
- **THEN** system converts to kg and stores in database as kg value

#### Scenario: Consistent data format
- **WHEN** querying database
- **THEN** all weight values are stored as double-precision floating point in kg
