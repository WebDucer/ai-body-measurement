using BodyMeasurement.Views;

namespace BodyMeasurement;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("addweight", typeof(AddEditWeightPage));
		Routing.RegisterRoute("editweight", typeof(AddEditWeightPage));
	}
}
