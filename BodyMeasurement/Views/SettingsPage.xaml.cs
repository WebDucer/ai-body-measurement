using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;

	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	private void OnLanguageChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (e.Value && sender is RadioButton radioButton)
		{
			_viewModel.SelectedLanguage = radioButton.Value?.ToString() ?? "en";
		}
	}

	private void OnUnitChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (e.Value && sender is RadioButton radioButton)
		{
			_viewModel.SelectedUnit = radioButton.Value?.ToString() ?? "kg";
		}
	}
}
