using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class ChartPage : ContentPage
{
	private readonly ChartViewModel _viewModel;

	public ChartPage(ChartViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadChartDataCommand.ExecuteAsync(null);
	}
}
