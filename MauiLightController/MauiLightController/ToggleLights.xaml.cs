namespace MauiLightController;

using Controller;
public partial class ToggleLights : ContentPage
{
    public ToggleLights()
    {
        InitializeComponent();
        CreateControl();
    }
    public class Light
    {
        public string Id { get; set; }
    }
    private void CreateControl()
    {
        DataTemplate dataTemplate = new DataTemplate(() =>
        {
            Button buttonOn = new Button()
            {
                FontSize = 24,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                BackgroundColor = Colors.Yellow
            };
            buttonOn.SetBinding(Button.TextProperty, "Id");
            buttonOn.Clicked += async (sender, args) => TurnLightOnClicked(sender, args, buttonOn.Text);

            Button buttonOff = new Button()
            {
                FontSize = 24,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                BackgroundColor = Colors.Purple
            };
            buttonOff.SetBinding(Button.TextProperty, "Id");
            buttonOff.Clicked += async (sender, args) => TurnLightOffClicked(sender, args, buttonOff.Text);

            Button buttonFade = new Button()
            {
                FontSize = 24,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                BackgroundColor = Colors.Green
            };
            buttonFade.SetBinding(Button.TextProperty, "Id");
            buttonFade.Clicked += async (sender, args) => FadeLightClicked(sender, args, buttonFade.Text);


            StackLayout horizontalStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            horizontalStackLayout.Add(buttonOn);
            horizontalStackLayout.Add(buttonOff);
            horizontalStackLayout.Add(buttonFade);

            return horizontalStackLayout;
        });
        StackLayout stackLayout = new StackLayout();
        BindableLayout.SetItemsSource(stackLayout, Controller.Lights);
        BindableLayout.SetItemTemplate(stackLayout, dataTemplate);

        ScrollView scrollView = new ScrollView
        {
            Margin = new Thickness(20),
            Content = stackLayout
        };

        Title = "ScrollView demo";
        Content = scrollView;
    }

    private async void TurnLightOnClicked(object sender, EventArgs e, string assetid)
    {
        Controller.Lights.Find(L => L.Id == assetid).Toggle();
    }
    private async void TurnLightOffClicked(object sender, EventArgs e, string assetid)
    {
        Controller.Lights.Find(L => L.Id == assetid).FadeLight();
    }
    private async void FadeLightClicked(object sender, EventArgs e, string assetid)
    {
        Controller.Lights.Find(L => L.Id == assetid).Reset();
    }
}