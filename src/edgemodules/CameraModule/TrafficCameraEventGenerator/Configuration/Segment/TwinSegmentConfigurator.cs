using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using NLog;
using Polly;
using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Transmitters;
using Message = Microsoft.Azure.Devices.Client.Message;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class TwinSegmentConfigurator : ITrafficSegmentConfigurator
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private DeviceClient _deviceClient;
        private readonly string _deviceConnectionString;

        private DeviceClient IoTHubClient => _deviceClient ??
                                             (_deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString));

        public TwinSegmentConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
            _deviceConnectionString = ConfigurationCache.GetValue("deviceConnectionString");
            _deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString);
            _deviceClient.SetDesiredPropertyUpdateCallbackAsync(TwinPropertiesChanged, null);
        }

        private Task TwinPropertiesChanged(TwinCollection desiredproperties, object usercontext)
        {
            OnConfigurationChanged(GetConfigurationFromTwin(desiredproperties));
            return Task.CompletedTask;
        }

        public async Task<TrafficSegmentConfiguration> GetConfiguration()
        {
            var iotHubOwnerConnectionString = _configurationReader.GetConfigValue<string>("IOTHUB_OWNER_CONNECTIONSTRING", true);

            try
            {
                var twin = await _deviceClient.GetTwinAsync();
                return GetConfigurationFromTwin(twin.Properties.Desired);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"An error occurred when trying read device on IoT Hub: {e.Message}");
                throw;
            }
        }

        private TrafficSegmentConfiguration GetConfigurationFromTwin(TwinCollection propertiesDesired)
        {
            return new TrafficSegmentConfiguration
            {
                AverageCarsPerMinute = (int)propertiesDesired["AverageCarsPerMinute"],
                CameraDistance = propertiesDesired["CameraDistance"],
                MaxSpeed = propertiesDesired["MaxSpeed"],
                MinSpeed = propertiesDesired["MinSpeed"],
                NumberOfLanes = propertiesDesired["NumberOfLanes"],
                RushHours = propertiesDesired["RushHours"],
                SegmentId = propertiesDesired["SegmentId"],
                SpeedLimit = propertiesDesired["SpeedLimit"],
                SpeedingPercentage = propertiesDesired["SpeedingPercentage"]
            };
        }

        private string GetIoTHubUri(string connectionString)
        {
            foreach (var option in connectionString.Split(';'))
            {
                if (option.Split('=')[0].Equals("HostName", StringComparison.InvariantCultureIgnoreCase))
                {
                    return option.Split('=')[1];
                }
            }

            return null;
        }

        protected virtual void OnConfigurationChanged(TrafficSegmentConfiguration e)
        {
            ConfigurationUpdated?.Invoke(this, e);
        }

        public event EventHandler<TrafficSegmentConfiguration> ConfigurationUpdated;
    }
}
