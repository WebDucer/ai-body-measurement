## MODIFIED Requirements

### Requirement: User can edit existing weight measurement
The system SHALL allow users to modify the weight value, date, or notes of an existing measurement. Navigation to the edit screen SHALL pass the measurement ID as a typed `int` via `ShellNavigationQueryParameters`; the edit ViewModel SHALL receive it via `IQueryAttributable` without string parsing.

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

#### Scenario: Edit navigation does not crash
- **WHEN** user taps "Edit" on any measurement
- **THEN** the app navigates to the edit screen without throwing an `InvalidCastException`

### Requirement: User can delete weight measurement
The system SHALL allow users to delete an existing weight measurement. The confirmation dialog SHALL be shown via `INavigationService.ShowConfirmationAsync`, with dialog text sourced from localized strings.

#### Scenario: Delete measurement with confirmation
- **WHEN** user selects delete action on a measurement and confirms
- **THEN** system permanently removes the measurement from storage

#### Scenario: Cancel delete operation
- **WHEN** user selects delete action on a measurement and cancels
- **THEN** system retains the measurement without changes

#### Scenario: Delete confirmation text is localized
- **WHEN** confirmation dialog appears
- **THEN** title and message text are displayed in the currently selected language

## ADDED Requirements

### Requirement: Measurement list empty state text is localized
The system SHALL display the empty state message in the currently selected language when no measurements exist.

#### Scenario: Empty state in English
- **WHEN** user has no measurements and app language is English
- **THEN** list shows the localized English empty state text

#### Scenario: Empty state in German
- **WHEN** user has no measurements and app language is German
- **THEN** list shows the localized German empty state text

### Requirement: Swipe action labels are localized
The system SHALL display "Edit" and "Delete" swipe action labels in the currently selected language.

#### Scenario: Swipe labels in English
- **WHEN** user swipes a measurement item and app language is English
- **THEN** action buttons show "Edit" and "Delete"

#### Scenario: Swipe labels in German
- **WHEN** user swipes a measurement item and app language is German
- **THEN** action buttons show the German equivalents
