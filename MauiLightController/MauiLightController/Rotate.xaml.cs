namespace MauiLightController;

using Controller;
using System.Diagnostics;

public partial class Rotate : ContentPage
{
    Controller controller;
    Stopwatch stopwatch;
    public bool Active = false;
    Dictionary<string, int> Lightpositions = new Dictionary<string, int>();
    public Rotate()
    {
        controller = new Controller();
        stopwatch = Stopwatch.StartNew();
        int i = 90;
        foreach(string light in controller.lights)
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
    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        // Update UI Label with compass state
        if (stopwatch.ElapsedMilliseconds > 100 && Active)
        {

            int rotation = (int)e.Reading.HeadingMagneticNorth;
            string mode = "Direction";
            if(mode == "Brightness")
            {
                int value = (int)((rotation - 180) / 180d * 255d);
                if (value < 0) value *= -1;
                CompassLabel.Text = value.ToString();
                controller.ChangeColor(controller.lights[0], new int[] { value, value, value });
            }else if(mode == "Direction")
            {
                foreach(KeyValuePair<string, int> pair in Lightpositions)
                {
                    if(pair.Value < rotation +15 && pair.Value > rotation - 15)
                    {
                        int value = (pair.Value - rotation) * 17;
                        if (value > 0) value *= -1;
                        value += 255;
                        controller.ChangeColor(pair.Key, new int[] { value, value, value });
                    }
                }
            }

            


            stopwatch.Restart();
        }

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