using TrafficCameraEventGenerator.Configuration.Settings;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class BusySegmentConsoleConfigurator : ITrafficSegmentConfigurator
    {

        public TrafficSegmentConfiguration GetConfiguration()
        {
            return TrafficSegmentConfiguration.Busy;
        }
    }
}
