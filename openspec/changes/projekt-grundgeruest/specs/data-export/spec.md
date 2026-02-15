# Specification: Data Export

## ADDED Requirements

### Requirement: User can export weight data to CSV format
The system SHALL allow users to export all weight measurements to a CSV (Comma-Separated Values) file.

#### Scenario: Export all measurements to CSV
- **WHEN** user selects CSV export option
- **THEN** system generates CSV file containing all weight measurements

#### Scenario: CSV file structure
- **WHEN** CSV file is generated
- **THEN** file contains headers: "Date", "Weight (kg)", "Weight (lbs)", "Notes"

#### Scenario: CSV date format
- **WHEN** exporting dates to CSV
- **THEN** system uses ISO 8601 format (YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS)

#### Scenario: CSV with measurements containing notes
- **WHEN** exporting measurements with notes
- **THEN** notes are properly escaped if they contain commas or quotes

#### Scenario: CSV with empty notes
- **WHEN** measurement has no notes
- **THEN** CSV includes empty field or blank value for notes column

#### Scenario: CSV includes both units
- **WHEN** generating CSV
- **THEN** file includes weight in both kg and lbs columns regardless of user's preferred unit

### Requirement: User can export weight data to JSON format
The system SHALL allow users to export all weight measurements to a JSON (JavaScript Object Notation) file.

#### Scenario: Export all measurements to JSON
- **WHEN** user selects JSON export option
- **THEN** system generates JSON file containing all weight measurements

#### Scenario: JSON file structure
- **WHEN** JSON file is generated
- **THEN** file contains "exportDate" (ISO timestamp) and "entries" array

#### Scenario: JSON entry format
- **WHEN** each measurement is exported to JSON
- **THEN** entry includes "date", "weightKg", "weightLbs", and "notes" fields

#### Scenario: JSON with null notes
- **WHEN** measurement has no notes
- **THEN** JSON entry includes "notes" field with null or empty string value

#### Scenario: JSON date format
- **WHEN** exporting dates to JSON
- **THEN** system uses ISO 8601 format with timezone (e.g., "2026-02-15T06:00:00Z")

### Requirement: Export files include all measurements
The system SHALL include all recorded weight measurements in export files without filtering or limit.

#### Scenario: Export with many measurements
- **WHEN** user has 500+ measurements and exports data
- **THEN** system includes all measurements in export file

#### Scenario: Export chronological order
- **WHEN** generating export file
- **THEN** measurements are ordered by date (oldest to newest or consistent order)

### Requirement: User can share or save exported files
The system SHALL provide platform-native sharing mechanism to save or share exported files.

#### Scenario: Share CSV file on mobile
- **WHEN** user exports to CSV on iOS or Android
- **THEN** system opens native share sheet to save or share file

#### Scenario: Save CSV file on desktop
- **WHEN** user exports to CSV on Windows or macOS
- **THEN** system opens save file dialog to choose location

#### Scenario: Share JSON file
- **WHEN** user exports to JSON
- **THEN** system provides same sharing/saving mechanism as CSV

#### Scenario: Default filename
- **WHEN** system prompts to save export file
- **THEN** filename includes format and date (e.g., "weight-data-2026-02-15.csv")

### Requirement: Export handles edge cases gracefully
The system SHALL handle edge cases during export without crashing or producing corrupt files.

#### Scenario: Export with no measurements
- **WHEN** user attempts export with no recorded measurements
- **THEN** system either prevents export with message or generates valid empty file structure

#### Scenario: Export with special characters in notes
- **WHEN** notes contain special characters (quotes, newlines, unicode)
- **THEN** system properly escapes characters to maintain valid CSV/JSON format

#### Scenario: Large file size warning
- **WHEN** export would generate very large file (e.g., 10+ years of daily data)
- **THEN** system proceeds with export without size restrictions (files should remain manageable)

### Requirement: Export files are valid and importable
The system SHALL generate export files that conform to standard CSV and JSON specifications.

#### Scenario: CSV parseable by Excel
- **WHEN** CSV file is opened in Microsoft Excel or Google Sheets
- **THEN** data loads correctly with proper column separation

#### Scenario: CSV parseable by programming libraries
- **WHEN** CSV file is parsed by standard CSV libraries (Python csv, C# CsvHelper, etc.)
- **THEN** data parses correctly without errors

#### Scenario: JSON parseable by standard parsers
- **WHEN** JSON file is parsed by standard JSON libraries
- **THEN** data deserializes correctly into expected structure

#### Scenario: JSON valid syntax
- **WHEN** JSON file is validated against JSON specification
- **THEN** file passes validation with no syntax errors

### Requirement: Export operation provides user feedback
The system SHALL provide clear feedback during export process.

#### Scenario: Export in progress indicator
- **WHEN** user initiates export
- **THEN** system displays loading indicator or progress message

#### Scenario: Export success confirmation
- **WHEN** export completes successfully
- **THEN** system displays success message or confirmation

#### Scenario: Export failure notification
- **WHEN** export fails (e.g., permission denied, disk full)
- **THEN** system displays error message with clear reason

### Requirement: Exported data maintains precision
The system SHALL export weight values with full precision to avoid data loss.

#### Scenario: Weight precision in CSV
- **WHEN** exporting to CSV
- **THEN** weight values include at least 1 decimal place (e.g., "75.5" not "75" or "76")

#### Scenario: Weight precision in JSON
- **WHEN** exporting to JSON
- **THEN** weight values maintain full floating-point precision from database
