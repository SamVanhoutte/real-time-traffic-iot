using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using NLog;
using Polly;
using TrafficCameraEventGenerator.Cars;

namespace TrafficCameraEventGenerator.Transmitters
{
    public class IoTEdgeModuleTransmitter : IEventTransmitter
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private ModuleClient _moduleClient;
        private string _outputName;

        private ModuleClient EdgeModuleClient
        {
            get
            {
                if (_moduleClient != null) return _moduleClient;

                MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
                ITransportSettings[] settings = { mqttSetting };

                // Open a connection to the Edge runtime
                _moduleClient = ModuleClient.CreateFromEnvironmentAsync(settings).Result;
                return _moduleClient;
            }
        }

        public IoTEdgeModuleTransmitter(string outputName = null)
        {
            _outputName = outputName ?? "camera";
        }
        public void Dispose()
        {
        }

        public async Task Transmit(CameraEvent cameraEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Debug($"Transmitting message in IoT Edge module to output {_outputName}");
                var policy = Policy
                    .Handle<TimeoutException>()
                    .Or<DeviceMessageLockLostException>()
                    .Or<IotHubCommunicationException>()
                    .Or<IotHubThrottledException>()
                    .Or<ServerBusyException>()
                    .WaitAndRetryAsync(5, retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, span, retryCount, context) => { _logger.Warn(exception, $"Retry {retryCount}/{5} sending to IoT Edge Module, because of exception {exception}"); });

                // Retry maximum 5 times with exponential backoff for the above exceptions
                await policy.ExecuteAsync(async (token) =>
                {
                    await EdgeModuleClient.SendEventAsync(_outputName, new Message(Encoding.UTF8.GetBytes(cameraEvent.ToJson())), token);
                }, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"An error occurred when trying to send message to IoT Hub: {e.Message}");
            }
        }
    }
}
