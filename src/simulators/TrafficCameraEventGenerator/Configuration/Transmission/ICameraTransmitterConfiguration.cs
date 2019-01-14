using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator.Configuration.Transmission
{
    public interface ICameraTransmitterConfigurator
    {
        IEventTransmitter CreateTransmitter(CameraType cameraId);
    }
}