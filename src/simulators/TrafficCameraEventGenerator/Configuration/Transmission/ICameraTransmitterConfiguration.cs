using System.Threading.Tasks;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator.Configuration.Transmission
{
    public interface ICameraTransmitterConfigurator
    {
        Task<IEventTransmitter> CreateTransmitter(string segmentId, CameraType cameraId);
    }
}