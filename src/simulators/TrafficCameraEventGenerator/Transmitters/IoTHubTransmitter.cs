using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using NLog;
using Polly;
using TrafficCameraEventGenerator.Cars;

namespace TrafficCameraEventGenerator.Transmitters
{
    public class IoTHubTransmitter : IEventTransmitter
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _deviceConnectionString;
        private DeviceClient _deviceClient;

        private DeviceClient IoTHubClient => _deviceClient ??
                                             (_deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString));

        public IoTHubTransmitter(string deviceConnectionString)
        {
            _deviceConnectionString = deviceConnectionString;
        }
        public void Dispose()
        {
        }

        public async Task Transmit(CameraEvent cameraEvent, CancellationToken cancellationToken)
        {
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
                await policy.ExecuteAsync(async (token) =>
                {
                    await IoTHubClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(cameraEvent.ToJson())), token);
                }, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"An error occurred when trying to send message to IoT Hub: {e.Message}");
            }
        }
    }
}
