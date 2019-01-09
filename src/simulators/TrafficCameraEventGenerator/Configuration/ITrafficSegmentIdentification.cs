namespace TrafficCameraEventGenerator.Configuration
{
    public interface ITrafficSegmentIdentification
    {
        string SegmentId { get; set; }
        ICameraTransmitterConfiguration InitialCamera { get; set; }
        ICameraTransmitterConfiguration LastCamera { get; set; }
    }
}
