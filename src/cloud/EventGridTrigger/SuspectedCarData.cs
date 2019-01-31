using System;

namespace EventGridTrigger
{
    public class SuspectedCarData
    {
        public string LicensePlate { get; set; }
        public string TrajectId { get; set; }
        public DateTime EventTime { get; set; }
        public string Country { get; set; }
        public string Make { get; set; }
        public string SuspectedOf { get; set; }
    }
}