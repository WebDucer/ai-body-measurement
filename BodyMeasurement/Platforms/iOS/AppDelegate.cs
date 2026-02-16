using Foundation;

namespace BodyMeasurement;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	public AppDelegate()
	{
		// Initialize SQLite for iOS
		SQLitePCL.Batteries_V2.Init();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
