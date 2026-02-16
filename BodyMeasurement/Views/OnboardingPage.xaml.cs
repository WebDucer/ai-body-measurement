using BodyMeasurement.Services;

namespace BodyMeasurement.Views;

public partial class OnboardingPage : ContentPage
{
	private readonly ISettingsService _settingsService;
	private int _currentPosition = 0;

	public OnboardingPage(ISettingsService settingsService)
	{
		InitializeComponent();
		_settingsService = settingsService;

		// Create onboarding screens
		OnboardingCarousel.ItemsSource = CreateOnboardingScreens();
		OnboardingCarousel.PositionChanged += OnPositionChanged;
	}

	private List<View> CreateOnboardingScreens()
	{
		return new List<View>
		{
			// Screen 1: Welcome
			CreateWelcomeScreen(),
			
			// Screen 2: Features
			CreateFeaturesScreen(),
			
			// Screen 3: Unit Selection
			CreateUnitSelectionScreen()
		};
	}

	private View CreateWelcomeScreen()
	{
		var subtitleLabel = new Label
		{
			Text = "Track your weight easily and privately with Body Measurement",
			FontSize = 16,
			HorizontalTextAlignment = TextAlignment.Center
		};
		subtitleLabel.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#666666"), Color.FromArgb("#AAAAAA"));

		return new VerticalStackLayout
		{
			Padding = new Thickness(32),
			Spacing = 24,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "👋",
					FontSize = 72,
					HorizontalOptions = LayoutOptions.Center
				},
				new Label
				{
					Text = "Welcome",
					FontSize = 32,
					FontAttributes = FontAttributes.Bold,
					HorizontalTextAlignment = TextAlignment.Center
				},
				subtitleLabel
			}
		};
	}

	private View CreateFeaturesScreen()
	{
		return new VerticalStackLayout
		{
			Padding = new Thickness(32),
			Spacing = 24,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "✨",
					FontSize = 72,
					HorizontalOptions = LayoutOptions.Center
				},
				new Label
				{
					Text = "Features",
					FontSize = 32,
					FontAttributes = FontAttributes.Bold,
					HorizontalTextAlignment = TextAlignment.Center
				},
				new VerticalStackLayout
				{
					Spacing = 12,
					Children =
					{
						new Label { Text = "📝 Record weight measurements", FontSize = 16, HorizontalTextAlignment = TextAlignment.Center },
						new Label { Text = "📊 View charts and trends", FontSize = 16, HorizontalTextAlignment = TextAlignment.Center },
						new Label { Text = "📈 Track your progress", FontSize = 16, HorizontalTextAlignment = TextAlignment.Center },
						new Label { Text = "💾 Export your data", FontSize = 16, HorizontalTextAlignment = TextAlignment.Center }
					}
				}
			}
		};
	}

	private View CreateUnitSelectionScreen()
	{
		var kgButton = new Button
		{
			Text = "Kilograms (kg)",
			TextColor = Colors.White,
			WidthRequest = 200
		};
		kgButton.SetAppThemeColor(Button.BackgroundColorProperty, Color.FromArgb("#2196F3"), Color.FromArgb("#1976D2"));
		kgButton.Clicked += OnKgSelected;

		var lbsButton = new Button
		{
			Text = "Pounds (lbs)",
			TextColor = Colors.White,
			WidthRequest = 200
		};
		lbsButton.SetAppThemeColor(Button.BackgroundColorProperty, Color.FromArgb("#2196F3"), Color.FromArgb("#1976D2"));
		lbsButton.Clicked += OnLbsSelected;

		var subtitleLabel = new Label
		{
			Text = "Select your preferred weight unit",
			FontSize = 16,
			HorizontalTextAlignment = TextAlignment.Center,
			Margin = new Thickness(0, 0, 0, 16)
		};
		subtitleLabel.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#666666"), Color.FromArgb("#AAAAAA"));

		return new VerticalStackLayout
		{
			Padding = new Thickness(32),
			Spacing = 24,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "⚖️",
					FontSize = 72,
					HorizontalOptions = LayoutOptions.Center
				},
				new Label
				{
					Text = "Choose Your Unit",
					FontSize = 32,
					FontAttributes = FontAttributes.Bold,
					HorizontalTextAlignment = TextAlignment.Center
				},
				subtitleLabel,
				new VerticalStackLayout
				{
					Spacing = 12,
					HorizontalOptions = LayoutOptions.Center,
					Children = { kgButton, lbsButton }
				}
			}
		};
	}

	private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
	{
		_currentPosition = e.CurrentPosition;

		// Update button text on last screen
		if (_currentPosition == 2) // Last screen (0-indexed)
		{
			BtnNext.Text = "Done";
		}
		else
		{
			BtnNext.Text = "Next";
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (_currentPosition < 2)
		{
			OnboardingCarousel.Position = _currentPosition + 1;
		}
		else
		{
			CompleteOnboarding();
		}
	}

	private void OnSkipClicked(object? sender, EventArgs e)
	{
		CompleteOnboarding();
	}

	private void OnKgSelected(object? sender, EventArgs e)
	{
		_settingsService.PreferredUnit = "kg";
	}

	private void OnLbsSelected(object? sender, EventArgs e)
	{
		_settingsService.PreferredUnit = "lbs";
	}

	private void CompleteOnboarding()
	{
		_settingsService.OnboardingCompleted = true;
		
		// Switch from OnboardingPage to AppShell
		var window = Application.Current?.Windows.FirstOrDefault();
		if (window != null)
		{
			window.Page = new AppShell();
		}
	}
}
