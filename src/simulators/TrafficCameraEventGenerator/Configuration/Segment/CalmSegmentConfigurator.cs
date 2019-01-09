namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class CalmSegmentConfigurator : ITrafficSegmentConfigurator
    {
        public TrafficSegmentConfiguration GetConfiguration()
        {
            return TrafficSegmentConfiguration.Calm;
        }
    }
}