﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class Light
    {
        public string Id { get;private set; }
        public string Name { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public int[] Color { get; private set; }
        public int WarmBrightness { get; private set; }
        public int ColdBrightness { get; private set; }
        public bool OnOff { get; private set; }

        public Thread Thread { get; private set; }
        private bool Working { get; set; }
        private bool Stop { get; set; }


        private string realm = "strijp";
        private string url = "https://staging.strijp.openremote.app";

        public Light(string id, string name, double longitude, double latitude)
        {
            Id = id;
            Name = name;
            Longitude = longitude;
            Latitude = latitude;

            Color = new int[] { 0, 0, 0 };
            WarmBrightness = 30;
            ColdBrightness = 30;
            OnOff = false;

            Working = false;
            Stop = false;
        }
        public void StopTasks()
        {
            if (Working)
            {
                Stop = true;
            }
            while (Stop) ;
        }
        public void ChangeColor(int[] color)
        {
            StopTasks();
            Thread = new Thread(() => changeColor(new int[] { 0, 0, 0 }));
            Thread.Start();
        }
        private async void changeColor(int[] color)
        {
            var client = new RestClient(url + "/api/" + realm + "/asset/" + Id + "/attribute/colourRgbLed");
            var request = new RestRequest();
            request.Method = Method.Put;
            request.AddBody(color);
            request.AddHeader("authorization", "Bearer " + Token.GetToken());
            client.ExecuteAsync(request);
            return;
        }

        public void Toggle()
        {
            StopTasks();
            Thread = new Thread(() => toggle(!OnOff));
            OnOff = !OnOff;
            Thread.Start();
        }
        private void toggle(bool onOff)
        {
            var client = new RestClient(url + "/api/" + realm + "/asset/" + Id + "/attribute/onOff");
            var request = new RestRequest();
            request.Method = Method.Put;
            request.AddBody(onOff);
            request.AddHeader("authorization", "Bearer " + Token.GetToken());
            Console.WriteLine(client.Execute(request).Content);
            return;
        }

        public void FadeLight()
        {
            StopTasks();
            Thread = new Thread(() => fadeLight());
            Thread.Start();
        }
        private async Task fadeLight()
        {
            Working = true;
            int[] Color = new int[] { 255, 0, 0 };
            while (Working)
            {
                changeColor(Color);
                Color = offset(Color,10);
                Thread.Sleep(10);
                if (Stop)
                {
                    Working = false;
                }
            }
            Stop = false;
            return;
        }



        int[] offset(int[] color, int offset)
        {
            for (int i = 0; i < color.Length; i++)
            {
                if (color[i] == 255)
                {
                    int after = i + 1;
                    int before = i - 1;
                    if (after == color.Length) after = 0;
                    if (before == -1) before = color.Length - 1;

                    if (color[before] == 0 && color[after] < color[i])
                    {
                        color[after] += offset;
                        if (color[after] > 255)
                        {
                            color[i] -= color[after] - 255;
                            color[after] = 255;
                        }
                    }
                    else
                    {
                        color[before] -= offset;
                        if (color[before] < 0)
                        {
                            color[after] -= color[before];
                            color[before] = 0;
                        }
                    }
                    break;
                }
            }
            return color;
        }

    }
}