using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator.Configuration.Transmission
{
    public class ConsoleTransmitterConfigurator : ICameraTransmitterConfigurator
    {
        private readonly IConfigurationReader _configurationReader;

        public ConsoleTransmitterConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        public Task<IEventTransmitter> CreateTransmitter(string segmentId, CameraType cameraId)
        {
            var configuredColor = _configurationReader.GetConfigValue<string>($"CONSOLE_{cameraId.ToString().ToUpper().Replace("-", "")}_COLOR", false);

            if (!Enum.TryParse(configuredColor, true, out ConsoleColor consoleColor))
            {
                consoleColor = cameraId == CameraType.Camera1 ? ConsoleColor.Green : ConsoleColor.Red;
            }

            return Task.FromResult<IEventTransmitter>(new ConsoleTransmitter
            {
                ConsoleColor = consoleColor
            });
        }
    }
}
