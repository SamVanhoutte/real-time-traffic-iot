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
    public class IoTHubTransmitterConfigurator : ICameraTransmitterConfigurator
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public IoTHubTransmitterConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        public async Task<IEventTransmitter> CreateTransmitter(string segmentId, CameraType cameraId)
        {
            var iotHubOwnerConnectionString = _configurationReader.GetConfigValue<string>("IOTHUB_OWNER_CONNECTIONSTRING", true);


            try
            {
                var policy = Policy
                    .Handle<TimeoutException>()
                    .Or<DeviceMessageLockLostException>()
                    .Or<IotHubCommunicationException>()
                    .Or<IotHubThrottledException>()
                    .Or<ServerBusyException>()
                    .WaitAndRetryAsync(5, retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, span, retryCount, context) => { _logger.Warn(exception, $"Retry {retryCount}/{5} sending to IoT Hub, because of exception {exception}"); });

                // Retry maximum 5 times with exponential backoff for the above exceptions
                return await policy.ExecuteAsync(async () =>
                {
                    var iotHubServiceClient = RegistryManager.CreateFromConnectionString(iotHubOwnerConnectionString);
                    string deviceId = $"{segmentId}-{cameraId}";
                    var camDevice = await iotHubServiceClient.GetDeviceAsync(deviceId)
                                    ?? await iotHubServiceClient.AddDeviceAsync(new Device(deviceId));
                    //savanh-traffic-camera.azure-devices.net
                    return new IoTHubTransmitter($"HostName={GetIoTHubUri(iotHubOwnerConnectionString)};DeviceId={deviceId};SharedAccessKey={camDevice.Authentication.SymmetricKey.PrimaryKey}");
                });
            }
            catch (Exception e)
            {
                _logger.Error(e, $"An error occurred when trying read device on IoT Hub: {e.Message}");
                throw;
            }
        }

        private string GetIoTHubUri(string connectionString)
        {
            //"HostName=savanh-traffic-camera.azure-devices.net;DeviceId=dev-camera-01;SharedAccessKey=nSWtl/1yw/cXebzMAsLalHyzFoBx6nBXCIMe5AzzgcY="
            foreach (var option in connectionString.Split(';'))
            {
                if (option.Split('=')[0].Equals("HostName", StringComparison.InvariantCultureIgnoreCase))
                {
                    return option.Split('=')[1];
                }
            }

            return null;
        }
    }
}
