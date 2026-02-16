using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class StatisticsPage : ContentPage
{
	private readonly StatisticsViewModel _viewModel;

	public StatisticsPage(StatisticsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadStatisticsCommand.ExecuteAsync(null);
	}
}
