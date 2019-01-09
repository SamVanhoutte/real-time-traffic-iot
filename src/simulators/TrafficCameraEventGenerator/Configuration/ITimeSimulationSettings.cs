using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficCameraEventGenerator.Configuration
{
    public interface ITimeSimulationSettings
    {
        int TimeSimulationAccelerator { get; set; }
    }
}
