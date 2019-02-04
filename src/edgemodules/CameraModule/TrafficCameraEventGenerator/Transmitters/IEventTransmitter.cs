using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;

namespace TrafficCameraEventGenerator.Transmitters
{
    public interface IEventTransmitter : IDisposable
    {
        Task Transmit(CameraEvent cameraEvent, CancellationToken cancellationToken);
    }
}
