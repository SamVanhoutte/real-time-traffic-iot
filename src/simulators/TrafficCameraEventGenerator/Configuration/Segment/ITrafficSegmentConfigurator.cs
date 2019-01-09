using System;
using System.Collections.Generic;
using System.Linq;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Configuration.Segment;
using TrafficCameraEventGenerator.Configuration.Settings;

namespace TrafficCameraEventGenerator.Configuration
{
    public interface ITrafficSegmentConfigurator
    {
        TrafficSegmentConfiguration GetConfiguration();
    }

}
