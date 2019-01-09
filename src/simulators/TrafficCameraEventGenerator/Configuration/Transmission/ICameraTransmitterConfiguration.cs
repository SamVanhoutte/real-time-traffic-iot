using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator.Configuration
{
    public interface ICameraTransmitterConfigurator
    {
        IEventTransmitter CreateTransmitter(CameraType cameraId);
    }
}