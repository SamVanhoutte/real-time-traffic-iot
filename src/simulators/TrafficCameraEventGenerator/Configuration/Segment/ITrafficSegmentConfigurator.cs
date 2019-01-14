﻿using System.Threading.Tasks;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public interface ITrafficSegmentConfigurator
    {
        Task<TrafficSegmentConfiguration> GetConfiguration();
    }

}
