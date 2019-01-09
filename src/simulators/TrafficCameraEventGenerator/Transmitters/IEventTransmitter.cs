using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;

namespace TrafficCameraEventGenerator.Transmitters
{
    public interface IEventTransmitter : IDisposable
    {
        Task Initalize(ICameraTransmitterConfiguration configuration);
        Task Transmit(CameraEvent cameraEvent, CancellationToken cancellationToken);
    }
}
