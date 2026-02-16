# Specification: Statistics and Analytics

## ADDED Requirements

### Requirement: User can view current weight
The system SHALL display the user's most recent weight measurement as the current weight.

#### Scenario: Display current weight
- **WHEN** user has recorded at least one measurement
- **THEN** system displays the most recent measurement as current weight

#### Scenario: No measurements recorded
- **WHEN** user has not recorded any measurements
- **THEN** system displays placeholder message indicating no current weight available

#### Scenario: Current weight in preferred unit
- **WHEN** displaying current weight
- **THEN** system shows value in user's preferred unit (kg or lbs)

### Requirement: User can view starting weight
The system SHALL display the user's first recorded weight measurement as the starting weight.

#### Scenario: Display starting weight
- **WHEN** user has recorded at least one measurement
- **THEN** system displays the earliest measurement as starting weight

#### Scenario: Starting weight in preferred unit
- **WHEN** displaying starting weight
- **THEN** system shows value in user's preferred unit (kg or lbs)

### Requirement: User can view weight change over time
The system SHALL calculate and display weight change (difference between current and starting weight) in both absolute and percentage terms.

#### Scenario: Display absolute weight change
- **WHEN** user's starting weight is 75 kg and current weight is 72.5 kg
- **THEN** system displays "-2.5 kg" as absolute change

#### Scenario: Display percentage weight change
- **WHEN** user's starting weight is 75 kg and current weight is 72.5 kg
- **THEN** system displays "-3.3%" as percentage change

#### Scenario: Weight increase
- **WHEN** current weight is higher than starting weight
- **THEN** system displays positive change with "+" prefix (e.g., "+2.0 kg" and "+2.7%")

#### Scenario: No weight change
- **WHEN** current weight equals starting weight
- **THEN** system displays "0.0 kg" and "0.0%"

#### Scenario: Single measurement only
- **WHEN** user has only one measurement
- **THEN** system displays "0.0 kg" and "0.0%" as no change occurred yet

### Requirement: User can view weight change for specific time periods
The system SHALL calculate and display weight change over predefined time periods (7 days, 30 days, 90 days).

#### Scenario: Weight change over 7 days
- **WHEN** user selects 7-day period
- **THEN** system calculates difference between current weight and weight 7 days ago

#### Scenario: Weight change over 30 days
- **WHEN** user selects 30-day period
- **THEN** system calculates difference between current weight and weight 30 days ago

#### Scenario: Weight change over 90 days
- **WHEN** user selects 90-day period
- **THEN** system calculates difference between current weight and weight 90 days ago

#### Scenario: Insufficient data for period
- **WHEN** user selects 30-day period but only has 10 days of data
- **THEN** system displays change from earliest available measurement with note indicating limited data range

#### Scenario: No measurement at exact period start
- **WHEN** calculating 30-day change but no measurement exists exactly 30 days ago
- **THEN** system uses the closest measurement before the 30-day mark

### Requirement: Statistics display visual indicators
The system SHALL display visual indicators (icons, colors) to quickly communicate weight trends.

#### Scenario: Weight decrease indicator
- **WHEN** weight has decreased
- **THEN** system displays downward arrow icon with the change

#### Scenario: Weight increase indicator
- **WHEN** weight has increased
- **THEN** system displays upward arrow icon with the change

#### Scenario: No change indicator
- **WHEN** weight has not changed
- **THEN** system displays neutral indicator (e.g., horizontal line or dash)

### Requirement: Statistics respect preferred unit
The system SHALL display all statistical calculations in the user's preferred unit with appropriate precision.

#### Scenario: Statistics in kilograms
- **WHEN** user's preferred unit is kg
- **THEN** all statistics display in kg with 1 decimal place (e.g., "72.5 kg")

#### Scenario: Statistics in pounds
- **WHEN** user's preferred unit is lbs
- **THEN** all statistics display in lbs with 1 decimal place (e.g., "159.8 lbs")

### Requirement: Statistics update in real-time
The system SHALL recalculate and update all statistics immediately when measurements change.

#### Scenario: Add measurement updates statistics
- **WHEN** user adds a new measurement
- **THEN** system immediately recalculates current weight, changes, and percentages

#### Scenario: Delete measurement updates statistics
- **WHEN** user deletes a measurement
- **THEN** system immediately recalculates statistics excluding deleted data

#### Scenario: Edit measurement updates statistics
- **WHEN** user edits a measurement value or date
- **THEN** system immediately recalculates all affected statistics

### Requirement: Percentage calculations use appropriate precision
The system SHALL calculate percentage changes with appropriate mathematical precision.

#### Scenario: Percentage calculation formula
- **WHEN** calculating percentage change
- **THEN** system uses formula: ((current - previous) / previous) × 100

#### Scenario: Percentage display precision
- **WHEN** displaying percentage change
- **THEN** system rounds to 1 decimal place (e.g., "3.3%" not "3.33333%")

#### Scenario: Handle division by zero
- **WHEN** starting weight is theoretically zero (edge case)
- **THEN** system handles gracefully without error and displays "N/A" or appropriate message
