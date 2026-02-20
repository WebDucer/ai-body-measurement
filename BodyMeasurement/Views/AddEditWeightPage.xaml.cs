using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class AddEditWeightPage : ContentPage
{
	public AddEditWeightPage(AddEditWeightViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
