using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class AddEditWeightPage : ContentPage
{
	private readonly AddEditWeightViewModel _viewModel;

	public AddEditWeightPage(AddEditWeightViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}
