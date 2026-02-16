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

		OnboardingCarousel.PositionChanged += OnPositionChanged;
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

	private async void CompleteOnboarding()
	{
		_settingsService.OnboardingCompleted = true;
		await Shell.Current.GoToAsync("///main");
	}
}
