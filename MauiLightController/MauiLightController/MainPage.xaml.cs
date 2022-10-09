namespace MauiLightController;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnToggleLightsClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("ToggleLights");
	}

    private async void OnRotateClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Rotate");
    }
}

