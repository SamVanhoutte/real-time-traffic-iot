using Arcus.EventGrid.Contracts;
using EventGridTrigger.EventData;
using GuardNet;

namespace EventGridTrigger.Events
{
    public class SuspectedCarDetectedEvent : Event<SuspectedCarData>
    {
        public SuspectedCarDetectedEvent()
        {
        }
        public SuspectedCarDetectedEvent(string id, string subject, SuspectedCarData eventData) : base(id, subject)
        {
            Guard.NotNull(eventData, nameof(eventData));
            Data = eventData;
        }

        public override string DataVersion { get; } = "1";
        public override string EventType { get; } = "Traffic.SuspectedCar";
    }
}