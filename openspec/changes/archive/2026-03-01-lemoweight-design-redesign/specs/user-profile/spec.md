## ADDED Requirements

### Requirement: User can set their name in Settings
The system SHALL allow users to enter their name in the Settings page. The value SHALL be stored persistently using the MAUI Preferences API.

#### Scenario: Name input field visible in Settings
- **WHEN** user opens the Settings page
- **THEN** a text input field for the user's name is visible in the Profile section

#### Scenario: Name persists across app restarts
- **WHEN** user enters a name and restarts the app
- **THEN** the name field in Settings and the home screen profile banner both show the entered name

#### Scenario: Name is optional
- **WHEN** user has not entered a name
- **THEN** the profile banner shows "Name" as placeholder text without errors

### Requirement: Home screen profile banner displays user name
The system SHALL display the user's configured name in the green profile banner on the home screen.

#### Scenario: User name shown in banner
- **WHEN** user has set a name in Settings
- **THEN** the profile banner below the logo circle shows the user's name

#### Scenario: Placeholder shown when name empty
- **WHEN** user has not set a name
- **THEN** the profile banner shows "Name" as placeholder text

### Requirement: Settings page has a Profile section
The system SHALL display a dedicated "Profile" section at the top of the Settings page containing both the name and goal weight fields.

#### Scenario: Profile section is the first section in Settings
- **WHEN** user opens the Settings page
- **THEN** the "Profile" section (with Name and Goal Weight fields) appears before the Language section
