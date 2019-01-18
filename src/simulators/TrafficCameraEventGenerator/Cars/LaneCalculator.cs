using System;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Configuration.Segment;

namespace TrafficCameraEventGenerator.Cars
{
    public class LaneCalculator
    {
        public static int CalculateLane(TrafficSegmentConfiguration segmentConfiguration, TrafficSegmentSituation segmentSituation, SimulatedCar car)
        {
            if (segmentConfiguration.NumberOfLanes > 2)
            {
                // do complexity
                if (car.Speeding && segmentSituation.IsRushHour(SimulatedClock.GetTimestamp(), out var rushHour))
                {
                    // Only take the two most left lanes
                    return new Random().Next(1, 3);
                }
            }
            return new Random().Next(1, segmentConfiguration.NumberOfLanes + 1);
        }
    }
}
