namespace MauiLightController;

using Controller;
using System.Diagnostics;

public partial class Rotate : ContentPage
{
    Stopwatch stopwatch;
    Stopwatch LocationTimer;
    public bool Active = false;
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;
    private double lon = 0;
    private double lat = 0;
    Dictionary<string, int> Lightpositions = new Dictionary<string, int>();
    public Rotate()
    {
        GetCurrentLocation();
        stopwatch = Stopwatch.StartNew();
        LocationTimer = Stopwatch.StartNew();
        int i = 90;
        foreach(string light in Controller.lights)
        {
            Lightpositions.Add(light, i);
            i += 30;
        }
        InitializeComponent();
        if (Compass.Default.IsSupported)
        {
            if (!Compass.Default.IsMonitoring)
            {
                // Turn on compass
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                Compass.Default.Start(SensorSpeed.UI);
            }
            else
            {
                // Turn off compass
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= Compass_ReadingChanged;
            }
        }
    }
    private async void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        // Update UI Label with compass state
        if (stopwatch.ElapsedMilliseconds > 500 && Active)
        {

            int rotation = (int)e.Reading.HeadingMagneticNorth;
            string mode = "rotate";
            if(mode == "Brightness")
            {
                int value = (int)((rotation - 180) / 180d * 255d);
                if (value < 0) value *= -1;
                CompassLabel.Text = value.ToString();
                Controller.ChangeColor(Controller.lights[0], new int[] { value, value, value });
            }else if(mode == "Direction")
            {
                foreach(KeyValuePair<string, int> pair in Lightpositions)
                {
                    if(pair.Value < rotation +15 && pair.Value > rotation - 15)
                    {
                        Controller.ChangeColor(pair.Key, new int[] { 255, 255, 255 });
                    }
                    else
                    {
                        Controller.ChangeColor(pair.Key, new int[] { 30, 0, 0 });
                    }
                }
            }else if(mode == "rotate")
            {
                if (!_isCheckingLocation && LocationTimer.ElapsedMilliseconds > 10000)
                {
                    Thread thread = new Thread(() => GetCurrentLocation()); 
                    thread.Start();
                }
                if (lon != 0 && lat != 0)
                {
                    foreach(Light light in Controller.Lights)
                    {
                        int angle = CalculateAngle(lon, lat, light.Longitude, light.Latitude);
                        if(angle < rotation + 15 && angle > rotation - 15)
                        {
                            Controller.ChangeColor(light.Id, new int[] { 255, 255, 255 });
                        }
                        else
                        {
                            Controller.ChangeColor(light.Id, new int[] { 30, 0, 0 });
                        }
                    }
                    CompassLabel.Text = "Angle To center: " +CalculateAngle(lon, lat, 5.458431811075331, 51.44583726090631).ToString() + "\n" + "Angle: " + rotation + "\n" + lon + "  "+lat ;
                }
            }
            stopwatch.Restart();
        }
    }

    private int CalculateAngle(double lon1, double lat1, double lon2, double lat2)
    {
        double angle = -400;
        double aanliggend = lat2 - lat1;
        double overstaand = lon2 - lon1;
        angle = Math.Atan(overstaand / aanliggend) * 180 / Math.PI;
        if (lat1 <= lat2)
        {
            if (angle < 0) angle += 360;
        }
        else
        {
            angle += 180;
        }

        return (int)angle;
    }

    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                lon =location.Longitude;
                lat =location.Latitude;
            }
                
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            // Unable to get location
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }
    private void OnActivateClicked(object sender, EventArgs e)
    {
        Active = !Active;
        if (Active)
        {
            ActivateBtn.Text = "Deactivate";
        }
        else
        {
            ActivateBtn.Text = "Activate";
        }
    }
}