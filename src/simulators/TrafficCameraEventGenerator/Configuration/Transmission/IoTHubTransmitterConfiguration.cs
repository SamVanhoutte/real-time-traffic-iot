using System;
using System.Collections.Generic;
using System.Text;
using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator.Configuration.Transmission
{
    public class IoTHubTransmitterConfigurator : ICameraTransmitterConfigurator
    {
        private readonly IConfigurationReader _configurationReader;

        public IoTHubTransmitterConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        public IEventTransmitter CreateTransmitter(CameraType cameraId)
        {
            return new IoTHubTransmitter(_configurationReader.GetConfigValue<string>($"IOTHUB_{cameraId.ToString().ToUpper()}_CONNECTION", true));
        }
    }
}
