# Specification: Data Visualization

## ADDED Requirements

### Requirement: User can view weight history as line chart
The system SHALL display weight measurements as a line chart showing weight trends over time.

#### Scenario: Display line chart with measurements
- **WHEN** user navigates to the chart view and has recorded measurements
- **THEN** system displays a line chart with date on X-axis and weight on Y-axis

#### Scenario: Empty chart state
- **WHEN** user navigates to chart view with no measurements
- **THEN** system displays empty state message prompting to add measurements

#### Scenario: Chart uses preferred unit
- **WHEN** user views chart with preferred unit set to lbs
- **THEN** system displays Y-axis values and data points in pounds

### Requirement: User can filter chart by time period
The system SHALL allow users to filter the displayed chart data by predefined time periods.

#### Scenario: Filter by 1 week
- **WHEN** user selects "1 Week" filter
- **THEN** system displays measurements from the last 7 days

#### Scenario: Filter by 1 month
- **WHEN** user selects "1 Month" filter
- **THEN** system displays measurements from the last 30 days

#### Scenario: Filter by 3 months
- **WHEN** user selects "3 Months" filter
- **THEN** system displays measurements from the last 90 days

#### Scenario: Filter by 6 months
- **WHEN** user selects "6 Months" filter
- **THEN** system displays measurements from the last 180 days

#### Scenario: View all data
- **WHEN** user selects "All" filter
- **THEN** system displays all recorded measurements

#### Scenario: Insufficient data for filter
- **WHEN** user selects "1 Month" filter but only has measurements from last 5 days
- **THEN** system displays available measurements (5 days) with appropriate message

### Requirement: Chart adapts to system theme
The system SHALL render charts that automatically adapt to light and dark mode based on system settings.

#### Scenario: Chart in light mode
- **WHEN** system is in light mode
- **THEN** chart displays with light background and dark line/text colors

#### Scenario: Chart in dark mode
- **WHEN** system is in dark mode
- **THEN** chart displays with dark background and light line/text colors

### Requirement: Chart displays appropriate scale
The system SHALL automatically calculate and display appropriate axis scales based on data range.

#### Scenario: Auto-scale Y-axis
- **WHEN** chart displays weight data ranging from 70kg to 75kg
- **THEN** Y-axis scales to show range with appropriate padding (e.g., 68kg to 77kg)

#### Scenario: Auto-scale X-axis
- **WHEN** chart displays filtered time period
- **THEN** X-axis scales to show selected time range with appropriate labels

### Requirement: Chart performance with large datasets
The system SHALL render charts efficiently even with large numbers of data points.

#### Scenario: Render 365 data points
- **WHEN** user has recorded daily measurements for a full year and selects "All" filter
- **THEN** system renders chart within 2 seconds without lag

#### Scenario: Smooth scrolling and interaction
- **WHEN** user interacts with chart (scrolling, filter changes)
- **THEN** system responds smoothly without UI freezing

### Requirement: User can view weight measurements as list
The system SHALL display weight measurements in a scrollable list format as an alternative to chart view.

#### Scenario: Display measurements list
- **WHEN** user navigates to list view
- **THEN** system displays all measurements with weight, date, and notes in reverse chronological order

#### Scenario: List item formatting
- **WHEN** viewing measurement in list
- **THEN** system displays weight in preferred unit, formatted date (e.g., "Feb 15, 2026"), and notes preview

#### Scenario: Scroll through long list
- **WHEN** user has many measurements
- **THEN** system provides smooth scrolling through entire list

### Requirement: List and chart views stay synchronized
The system SHALL keep list and chart views synchronized with the same underlying data.

#### Scenario: Add measurement updates both views
- **WHEN** user adds new measurement
- **THEN** both chart and list views reflect the new data immediately

#### Scenario: Delete measurement updates both views
- **WHEN** user deletes a measurement from list view
- **THEN** chart view updates to remove the deleted data point

#### Scenario: Edit measurement updates both views
- **WHEN** user edits a measurement
- **THEN** both chart and list views reflect the updated values
