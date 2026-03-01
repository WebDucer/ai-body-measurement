## ADDED Requirements

### Requirement: Profile and goal weight labels are localized
The system SHALL provide localized strings for all new UI text added in the Profile section of Settings and the home screen redesign, in both English and German.

#### Scenario: Profile section heading localized
- **WHEN** user views the Settings page in any supported language
- **THEN** the "Profile" section heading displays in the selected language

#### Scenario: Name field label localized
- **WHEN** user views the name input field in Settings
- **THEN** the label displays "Name" (EN) or "Name" (DE)

#### Scenario: Goal weight field label localized
- **WHEN** user views the goal weight field in Settings
- **THEN** the label displays "Goal Weight (kg)" (EN) or "Zielgewicht (kg)" (DE)

### Requirement: Home screen redesign strings are localized
The system SHALL provide localized strings for all text displayed in the redesigned home screen including motivational messages, progress bar labels, and measurement card labels.

#### Scenario: Motivational message localized
- **WHEN** home screen is displayed in German
- **THEN** the weight lost message reads "Du hast X kg verloren"

#### Scenario: Motivational message localized in English
- **WHEN** home screen is displayed in English
- **THEN** the weight lost message reads "You lost X"

#### Scenario: Motivational subtitle localized
- **WHEN** the motivational section is shown in German
- **THEN** subtitle reads "Whoo-hoo! Fliege weiter so und du landest wie geplant."

#### Scenario: Progress bar labels localized
- **WHEN** progress bar is displayed in German
- **THEN** "Start" label reads "Start" and goal label reads "Ziel"

#### Scenario: Progress bar labels in English
- **WHEN** progress bar is displayed in English
- **THEN** "Start" label reads "Start" and goal label reads "Goal"

#### Scenario: Measurement card labels localized in German
- **WHEN** home screen is displayed in German
- **THEN** cards show "Gewicht", "Viszeralfett", "Wasser in Körper", "Muskel", "BMI"

#### Scenario: Measurement card labels localized in English
- **WHEN** home screen is displayed in English
- **THEN** cards show "Weight", "Visceral Fat", "Water in Body", "Muscle", "BMI"
