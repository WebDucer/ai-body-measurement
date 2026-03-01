## ADDED Requirements

### Requirement: Home screen shows a custom branded header bar
The system SHALL display a custom top bar on the home screen containing the LemoWeight logo and app name on the left, and a person icon and gear icon on the right. The Shell navigation bar SHALL be hidden for this page.

#### Scenario: Header visible on launch
- **WHEN** user opens the app and the home screen is displayed
- **THEN** the custom header bar is visible with the LemoWeight logo, "LemoWeight" text, and two icon buttons

#### Scenario: Gear icon navigates to Settings
- **WHEN** user taps the gear icon in the header
- **THEN** the app navigates to the Settings tab

#### Scenario: Person icon navigates to Settings
- **WHEN** user taps the person icon in the header
- **THEN** the app navigates to the Settings tab

### Requirement: Home screen shows a green profile banner
The system SHALL display a green-background banner below the header containing a circular logo placeholder and the user's name.

#### Scenario: Profile banner shows user name
- **WHEN** the user has entered a name in Settings
- **THEN** the profile banner displays the user's name below the circular logo

#### Scenario: Profile banner shows placeholder when name not set
- **WHEN** the user has not entered a name in Settings
- **THEN** the profile banner displays "Name" as placeholder text

#### Scenario: Profile banner background color
- **WHEN** the profile banner is displayed
- **THEN** its background color is LemoWeight green (`#8CB43E`)

### Requirement: Home screen shows a motivational progress section
The system SHALL display a motivational message and a gradient progress bar between the profile banner and the measurement cards.

#### Scenario: Weight lost message shown
- **WHEN** the user has at least one measurement and a starting weight
- **THEN** the motivational section shows "Du hast X kg verloren" (or English equivalent)

#### Scenario: Motivational subtitle shown
- **WHEN** the motivational section is displayed
- **THEN** a subtitle encouragement message is shown below the main text

#### Scenario: Progress bar shows position between start and goal
- **WHEN** starting weight, current weight, and goal weight are all set
- **THEN** the progress bar marker is positioned proportionally at `(startWeight - currentWeight) / (startWeight - goalWeight)`, clamped to [0, 1]

#### Scenario: Progress bar shows zero progress when no goal set
- **WHEN** goal weight is not configured in Settings
- **THEN** the progress bar marker is positioned at 0 (start)

#### Scenario: Progress bar uses red-to-green gradient
- **WHEN** the progress bar is displayed
- **THEN** it renders as a horizontal gradient from red (left/start) through yellow to green (right/goal)

#### Scenario: Start and goal weight labels shown
- **WHEN** the progress section is visible
- **THEN** "Start" and "Ziel" labels appear above the bar ends, and starting weight / goal weight values appear below

#### Scenario: Goal weight label shows placeholder when not set
- **WHEN** goal weight is not configured
- **THEN** goal weight label shows "--"

### Requirement: Home screen shows body measurement overview cards
The system SHALL display five body measurement cards below the progress section: Gewicht (Weight), Viszeralfett, Wasser in Körper, Muskel, and BMI.

#### Scenario: Weight card shows current weight
- **WHEN** the user has at least one weight measurement
- **THEN** the Gewicht card displays the most recent weight value with unit

#### Scenario: Weight card shows placeholder when no data
- **WHEN** the user has no weight measurements
- **THEN** the Gewicht card displays "--"

#### Scenario: Placeholder cards show dash
- **WHEN** the Viszeralfett, Wasser, Muskel, or BMI cards are displayed
- **THEN** they show "--" as their value (placeholder for future tracking)

#### Scenario: Each card has a chevron button
- **WHEN** any measurement card is displayed
- **THEN** a green chevron-down icon button is visible on the right side of the card

#### Scenario: Cards have green border
- **WHEN** measurement cards are displayed
- **THEN** each card has a green border stroke color

### Requirement: Home screen retains the floating action button
The system SHALL continue to display a floating action button (FAB) in the bottom-right corner of the home screen for adding new measurements.

#### Scenario: FAB visible and functional
- **WHEN** the home screen is displayed
- **THEN** a green circular FAB with "+" is visible and tapping it opens the Add Measurement screen
