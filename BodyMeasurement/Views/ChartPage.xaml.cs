using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class ChartPage : ContentPage
{
	private readonly ChartViewModel _viewModel;
	private bool _isFirstLoad = true;

	public ChartPage(ChartViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		// Workaround for Syncfusion chart rendering issue:
		// Hide chart during data load, then show it after data is ready
		if (_isFirstLoad)
		{
			_isFirstLoad = false;
			// Small delay to let the page fully render first
			await Task.Delay(100);
		}
		
		await _viewModel.LoadChartDataCommand.ExecuteAsync(null);
	}
}
