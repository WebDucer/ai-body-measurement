using BodyMeasurement.ViewModels;

namespace BodyMeasurement.Views;

public partial class AddEditWeightPage : ContentPage, IQueryAttributable
{
	private readonly AddEditWeightViewModel _viewModel;

	public AddEditWeightPage(AddEditWeightViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		_viewModel.ApplyQueryAttributes(query);
	}
}
