using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;

namespace TrafficCameraEventGenerator.Transmitters
{
    public class ConsoleTransmitter : IEventTransmitter
    {
        public void Dispose()
        {
        }

        public Task Initalize(ICameraTransmitterConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public Task Transmit(CameraEvent cameraEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine(cameraEvent.ToJson());
            return Task.CompletedTask;
        }
    }
}
