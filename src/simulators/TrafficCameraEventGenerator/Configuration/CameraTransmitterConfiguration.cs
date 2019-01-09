using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficCameraEventGenerator.Configuration
{
    public class CameraTransmitterConfiguration : ICameraTransmitterConfiguration
    {
        public CameraTransmitterConfiguration(string connectionDetails)
        {
            ConnectionDetails = connectionDetails;
        }
        public string ConnectionDetails { get; set; }
    }
}
