using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using TrafficCameraEventGenerator.Cars;

namespace TrafficCameraEventGenerator.Transmitters
{
    public class IoTHubTransmitter : IEventTransmitter
    {
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
            await IoTHubClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(cameraEvent.ToJson())), cancellationToken);
        }
    }
}
