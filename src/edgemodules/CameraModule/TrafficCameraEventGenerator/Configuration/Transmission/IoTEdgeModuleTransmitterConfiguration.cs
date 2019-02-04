

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;
using NLog;
using Polly;
using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator.Configuration.Transmission
{
    public class IoTEdgeModuleTransmitterConfiguration : ICameraTransmitterConfigurator
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public IoTEdgeModuleTransmitterConfiguration(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        public async Task<IEventTransmitter> CreateTransmitter(string segmentId, CameraType cameraId)
        {
            try
            {
                return new IoTEdgeModuleTransmitter(_configurationReader.GetConfigValue("EDGE_MODULE_OUTPUT", false, cameraId.ToString()));
            }
            catch (Exception e)
            {
                _logger.Error(e, $"An error occurred when trying to create iot edge transmitter: {e.Message}");
                throw;
            }
        }
    }
}
