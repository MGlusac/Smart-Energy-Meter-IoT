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

        public async void SendMessage(string message)
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
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // Light Sensor
            var light = _hat.GetLightLevel();            

            //Direct Heat sensor
            var directHeat = _hat.ReadAnalog(FEZHAT.AnalogPin.Ain1);
           
            //The light is on
            Debug.WriteLine(directHeat);
            if (light > 0.5 && directHeat <= 0.74)
            {
                var msg = new
                {
                    timecreated = DateTime.UtcNow.ToString("o"),
                    payload = "Energy waste!"
                };

                SendMessage(msg.ToString());
            }
        }
    }
}