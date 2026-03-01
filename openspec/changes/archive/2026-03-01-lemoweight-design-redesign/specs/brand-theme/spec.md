## ADDED Requirements

### Requirement: App uses LemoWeight green color palette
The system SHALL display all primary UI elements (buttons, tab bar active state, FAB, progress indicators, borders, links) using the LemoWeight olive-lime green color (`#8CB43E`) instead of the previous purple palette.

#### Scenario: Primary button color
- **WHEN** any primary action button is visible
- **THEN** its background color is LemoWeight green (`#8CB43E`)

#### Scenario: Tab bar selected item color
- **WHEN** a tab is selected in the bottom navigation bar
- **THEN** the tab icon and label render in LemoWeight green

#### Scenario: FAB color
- **WHEN** the floating action button is visible on the home screen
- **THEN** its background color is LemoWeight green

#### Scenario: Activity indicator color
- **WHEN** a loading spinner is displayed
- **THEN** it renders in LemoWeight green

### Requirement: Color palette updates cascade app-wide via resource dictionary
The system SHALL define brand colors in `Colors.xaml` such that updating the resource dictionary is sufficient to change themed elements across all pages without per-file edits.

#### Scenario: Colors.xaml drives all themed elements
- **WHEN** the `Primary` color key in `Colors.xaml` is changed
- **THEN** all controls that reference `{StaticResource Primary}` or `{AppThemeBinding Light={StaticResource Primary}}` update automatically

### Requirement: Dark mode colors remain harmonious with the green theme
The system SHALL provide dark-mode variants of the green palette (`PrimaryDark`) such that the app looks coherent in both light and dark appearance modes.

#### Scenario: Dark mode primary color
- **WHEN** device is in dark mode
- **THEN** primary interactive elements use the dark-mode green variant (`#6E9E2D`)
