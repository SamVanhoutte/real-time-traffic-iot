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
        private string _deviceConnectionString;

        private DeviceClient IoTHubClient => _deviceClient ??
                                             (_deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString));

        public TwinSegmentConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        private Task TwinPropertiesChanged(TwinCollection desiredproperties, object usercontext)
        {
            _logger.Info($"Desired properties have been updated.");
            OnConfigurationChanged(GetConfigurationFromTwin(desiredproperties));
            return Task.CompletedTask;
        }

        public async Task<TrafficSegmentConfiguration> GetConfiguration()
        {
            _deviceConnectionString = ConfigurationCache.GetValue("deviceConnectionString");
            _deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString);
            await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(TwinPropertiesChanged, null);

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
                AverageCarsPerMinute = GetDesiredProperty("AverageCarsPerMinute", propertiesDesired, 10),
                CameraDistance = GetDesiredProperty("CameraDistance", propertiesDesired, 1000),
                MaxSpeed = GetDesiredProperty("MaxSpeed", propertiesDesired, 160),
                MinSpeed = GetDesiredProperty("MinSpeed", propertiesDesired, 50),
                NumberOfLanes = GetDesiredProperty("NumberOfLanes", propertiesDesired, 3),
                RushHours = TimePeriod.ParseList(GetDesiredProperty("RushHours", propertiesDesired, "07:00-08:00,17:00-18:00")),
                SegmentId = GetDesiredProperty("SegmentId", propertiesDesired, ""),
                SpeedLimit = GetDesiredProperty("SpeedLimit", propertiesDesired, 120),
                SpeedingPercentage = GetDesiredProperty("SpeedingPercentage", propertiesDesired, 2),
            };
        }

        private T GetDesiredProperty<T>(string propertyName, TwinCollection properties, T defaultValue = default(T))
        {
            if (properties.Contains(propertyName))
                return properties[propertyName];
            return defaultValue;
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
