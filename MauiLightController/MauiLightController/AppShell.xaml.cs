namespace MauiLightController;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("ToggleLights", typeof(ToggleLights));
        Routing.RegisterRoute("Rotate", typeof(Rotate));
    }
}
