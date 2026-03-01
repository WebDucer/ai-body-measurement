## ADDED Requirements

### Requirement: User can set a goal weight in Settings
The system SHALL allow users to enter a target/goal weight in the Settings page. The value SHALL be stored persistently using the MAUI Preferences API.

#### Scenario: Goal weight entry field visible in Settings
- **WHEN** user opens the Settings page
- **THEN** a numeric input field for goal weight is visible in the Profile section

#### Scenario: Goal weight persists across app restarts
- **WHEN** user enters a goal weight and restarts the app
- **THEN** the goal weight field in Settings still shows the entered value

#### Scenario: Goal weight accepts numeric input only
- **WHEN** user taps the goal weight field
- **THEN** a numeric keyboard is presented

#### Scenario: Goal weight stored in preferred unit context
- **WHEN** user enters a goal weight
- **THEN** the value is treated as kilograms (matching the app's internal storage unit)

### Requirement: Home screen reflects progress toward goal weight
The system SHALL compute and display a progress value on the home screen representing how far the user has moved from their starting weight toward their goal weight.

#### Scenario: Progress calculated from start to goal
- **WHEN** starting weight, current weight, and goal weight are all available
- **THEN** progress = clamp((startWeight - currentWeight) / (startWeight - goalWeight), 0, 1)

#### Scenario: Progress clamped to valid range
- **WHEN** current weight has passed below the goal weight
- **THEN** progress is shown as 1.0 (100%)

#### Scenario: No progress when goal not set
- **WHEN** goal weight has not been configured
- **THEN** progress value is 0 and progress bar marker is at the start position

### Requirement: Goal weight is optional
The system SHALL NOT require a goal weight to function. All home screen sections SHALL degrade gracefully when goal weight is absent.

#### Scenario: App functions without goal weight
- **WHEN** user has not set a goal weight
- **THEN** home screen displays current weight and measurements without errors; goal label shows "--"
