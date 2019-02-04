using System;
using Newtonsoft.Json;

namespace TrafficCameraEventGenerator.Cars
{
    public class CameraEvent
    {
        public SimulatedCar Car { get; set; }
        public DateTime EventTime { get; set; }
        public string CameraId { get; set; }
        public string TrajectId { get; set; }
        public int Lane { get; set; }

        public string ToJson()
        {
            var eventMessage = new
            {
                TrajectId,
                CameraId,
                EventTime,
                Lane,
                Car.Country,
                Car.LicensePlate,
                Car.Make,
                Car.Color
            };
            return JsonConvert.SerializeObject(eventMessage);
        }
    }
}
