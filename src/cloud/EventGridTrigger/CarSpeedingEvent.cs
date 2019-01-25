using System;
using System.Collections.Generic;
using System.Text;
using Arcus.EventGrid.Contracts;
using GuardNet;

namespace EventGridTrigger
{
    public class CarSpeedingEvent : Event<CarSpeedingData>
    {
        public CarSpeedingEvent()
        {
        }
        public CarSpeedingEvent(string id, string subject, CarSpeedingData eventData) : base(id, subject)
        {
            Guard.NotNull(eventData, nameof(eventData));
            Data = eventData;
        }
        public CarSpeedingEvent(string id, string subject, string licensePlate, string trajectId, DateTime detectionTime, int lane, string country, string make, string color, double speed, int limit) : base(id, subject)
        {
            Guard.NotNullOrWhitespace(licensePlate, nameof(licensePlate));

            Data.LicensePlate = licensePlate;
            Data.SpeedLimit = limit;
            Data.Color = color;
            Data.Country = country;
            Data.DetectionTime = detectionTime;
            Data.Lane = lane;
            Data.LicensePlate = licensePlate;
            Data.Make = make;
            Data.Speed = speed;
            Data.TrajectId = trajectId;
        }

        public override string DataVersion { get; } = "1";
        public override string EventType { get; } = "Traffic.SpeedingCarx";
    }
}
