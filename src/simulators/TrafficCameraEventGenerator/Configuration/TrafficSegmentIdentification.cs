using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TrafficSegmentIdentification: ITrafficSegmentIdentification
    {
        public string SegmentId { get; set; }
        public ICameraTransmitterConfiguration InitialCamera { get; set; }
        public ICameraTransmitterConfiguration LastCamera { get; set; }
    }
}
