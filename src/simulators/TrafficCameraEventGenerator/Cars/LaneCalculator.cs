using System;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Configuration.Segment;

namespace TrafficCameraEventGenerator.Cars
{
    public class LaneCalculator
    {
        public static int CalculateLane(TrafficSegmentConfiguration segmentConfiguration, SimulatedCar car)
        {
            if (segmentConfiguration.NumberOfLanes > 2)
            {
                // do complexity
                if (car.Speeding && segmentConfiguration.IsRushHour(SimulatedClock.GetTimestamp()))
                {
                    // Only take the two most left lanes
                    return new Random().Next(1, 3);
                }
            }
            return new Random().Next(1, segmentConfiguration.NumberOfLanes + 1);
        }
    }
}
