using System;
using System.Collections.Generic;
using System.Text;

namespace Savanh.Extensions.Randoms
{
    public class SimulatedClock
    {
        private static DateTime _physicalStartTime;
        private static DateTime _initialTime;
        private static int _multiplier;

        public static void Init(int simulationMultiplier = 1)
        {
            _initialTime = DateTime.Now;
            _physicalStartTime = DateTime.Now;
            _multiplier = simulationMultiplier;
        }
        public static void Init(int simulationMultiplier, DateTime initialTime)
        {
            _initialTime = initialTime;
            _physicalStartTime = DateTime.Now;
            _multiplier = simulationMultiplier;
        }

        public static DateTime GetTimestamp()
        {
            // Calculate timespan since start in milliseconds
            var runningTime = DateTime.Now.Subtract(_physicalStartTime).TotalMilliseconds;
            // Apply simulation multiplier if available
            if (_multiplier > 1)
            {
                runningTime = runningTime * _multiplier;
            }
            // Return simulated time
            return _initialTime.AddMilliseconds(runningTime);
        }
    }
}
