using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TimeSimulationSettings : ITimeSimulationSettings
    {
        public int TimeSimulationAccelerator { get; set; } = 1;
    }
}
