using System;
using System.Collections.Generic;
using System.Text;
using TrafficCameraEventGenerator.Configuration.Simulation;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TimeSimulationSettings : ITimeSimulationSettings
    {
        public int TimeSimulationAccelerator { get; set; } = 1;
    }
}
