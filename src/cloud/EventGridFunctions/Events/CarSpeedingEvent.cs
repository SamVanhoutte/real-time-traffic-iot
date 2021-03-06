﻿using System;
using System.Collections.Generic;
using System.Text;
using Arcus.EventGrid.Contracts;
using GuardNet;
using EventGridTrigger.EventData;

namespace EventGridTrigger.Events
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

        public override string DataVersion { get; } = "1";
        public override string EventType { get; } = "Traffic.SpeedingCar";
    }
}
