using System;
using System.Diagnostics;
using System.Text;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using GHIElectronics.UWP.Shields;
using Microsoft.Azure.Devices.Client;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartHome_IoT
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private FEZHAT _hat;
        private DispatcherTimer _timer;

        private readonly DeviceClient _deviceClient = DeviceClient.CreateFromConnectionString(
            "HostName=smart-home-IoT.azure-devices.net;DeviceId=workshop-pi;SharedAccessKey=Ryc6olM/TYJeQZlbHLljl5JLF+1hJ/FuZKBFYdMDHi0=");

        public MainPage()
        {
            InitializeComponent();

            // Initialize FEZ HAT
            SetupHat();
        }

        private async void SendMessage(string message)
        {
            // Send message to an IoT Hub using IoT Hub SDK
            try
            {
                var content = new Message(Encoding.UTF8.GetBytes(message));
                await _deviceClient.SendEventAsync(content);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception when sending message:" + e.Message);
            }
        }

        private async void SetupHat()
        {
            _hat = await FEZHAT.CreateAsync();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // Store light sensor value
            var light = _hat.GetLightLevel();

            // Store motion sensor value
            var directHeat = _hat.ReadAnalog(FEZHAT.AnalogPin.Ain1);
           
            // if the current room is empty and lights are on
            if (light > 0.2 && directHeat <= 0.7)
            {
                // Generate event message
                var msg = new
                {
                    deviceid = "119",
                    timecreated = DateTime.UtcNow.ToString("o"),
                    timespan = "1 min",
                    message = "Energy waste!"
                };

                // Send the message
                SendMessage(msg.ToString());
            }
        }
    }
}