using Android.App;
using Android.Runtime;

namespace BodyMeasurement;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
		// Initialize SQLite for Android
		SQLitePCL.Batteries_V2.Init();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
