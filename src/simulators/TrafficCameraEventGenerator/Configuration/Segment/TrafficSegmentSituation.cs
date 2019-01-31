using System;
using System.Collections.Generic;
using System.Linq;
using Alphirk.Simulation;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class TrafficSegmentSituation
    {
        private readonly TrafficSegmentConfiguration _configuration;
        private DateTime _currentEventStartTime;
        private TimeSpan _eventLength;
        private EventType _currentEvent = EventType.None;
        private TrafficTrend _currentTrend = TrafficTrend.None;

        public TrafficSegmentSituation(TrafficSegmentConfiguration configuration)
        {
            _configuration = configuration;
            MaxSpeed = configuration.MaxSpeed;
            MinSpeed = configuration.MinSpeed;
            AverageCarsPerMinute = configuration.AverageCarsPerMinute;
            SpeedingPercentage = configuration.SpeedingPercentage;
            SpeedLimit = configuration.SpeedLimit;
            RushHours = configuration.RushHours;
        }


        public void Resimulate(Random random)
        {
            // Check rush hour (busy or finished)
            if (IsRushHour(SimulatedClock.Time, out var currentRushHour))
            {
                AverageCarsPerMinute += random.Next(0, 10);
                try{
                    var rushHourEvolution = (currentRushHour.TimeLeft(SimulatedClock.Time).TotalSeconds /
                                         currentRushHour.Duration.TotalSeconds);
                    _currentTrend = rushHourEvolution < 0.80 ? TrafficTrend.Congesting : TrafficTrend.Clearing;
                }
                catch(DivideByZeroException)
                {
                    _currentTrend = TrafficTrend.Clearing;
                }   
                // First 80% of the timeframe, rushhour congestion, then clearing
                _currentEvent = EventType.RushHour;
            }
            else
            {
                if (_currentEvent == EventType.RushHour)
                {
                    // Rushhour done , so reset to normal!
                    ResetToNormal();
                }
            }

            
            if (_currentEvent == EventType.None)
            {
                // Apply random small changes
                AverageCarsPerMinute += random.Next(-2, 3);
                SpeedingPercentage = _configuration.SpeedingPercentage + random.Next(-1, 2);
                MinSpeed = _configuration.MinSpeed + random.Next(-5, 6);
                MaxSpeed = _configuration.MaxSpeed + random.Next(-3, 4);
                AverageSpeed = _configuration.SpeedLimit - random.Next(2, 10);

                // Allow to cause an accident
                if (random.Next(0, 2000) == 0)
                {
                    _currentEvent = EventType.Accident;
                    _currentEventStartTime = SimulatedClock.Time;
                    _eventLength = TimeSpan.FromMinutes(random.Next(15, 75));
                }
            }

            // Apply accident logic
            if (_currentEvent == EventType.Accident)
            {
                if (_currentEventStartTime.Add(_eventLength) <= SimulatedClock.Time)
                {
                    _currentEvent = EventType.None;
                    ResetToNormal();
                }
                // Apply random small changes
                AverageCarsPerMinute += random.Next(-2, 3);

                try{
                    var accidentEvolution = ((SimulatedClock.Time - _currentEventStartTime).TotalSeconds / _eventLength.TotalSeconds);
                    _currentTrend = accidentEvolution < 0.88 ? TrafficTrend.Congesting : TrafficTrend.Clearing;
                }
                catch(DivideByZeroException)
                {
                    _currentTrend = TrafficTrend.Clearing;
                } 
                _currentEvent = EventType.RushHour;
            }

            switch (_currentTrend)
            {
                case TrafficTrend.Clearing:
                    AverageSpeed += random.Next(2, 10);
                    break;
                case TrafficTrend.Congesting:
                    AverageSpeed -= random.Next(2, 10);
                    break;
            }

            GuardValues();
        }

        private void ResetToNormal()
        {
            _currentEvent = EventType.None;
            MaxSpeed = _configuration.MaxSpeed;
            MinSpeed = _configuration.MinSpeed;
            AverageCarsPerMinute = _configuration.AverageCarsPerMinute;
            SpeedingPercentage = _configuration.SpeedingPercentage;
            SpeedLimit = _configuration.SpeedLimit;
        }

        private void GuardValues()
        {
            // Guard thresholds
            if (SpeedingPercentage < _configuration.SpeedingPercentage - 3)
                SpeedingPercentage = _configuration.SpeedingPercentage - 3;
            if (SpeedingPercentage > _configuration.SpeedingPercentage + 3)
                SpeedingPercentage = _configuration.SpeedingPercentage + 3;
            if (MinSpeed < 5) MinSpeed = 5;
            if (MaxSpeed > 202) MaxSpeed = 202;
            if (AverageSpeed < MinSpeed + 5)
            {
                AverageSpeed = MinSpeed + 5;
            }

            if (AverageSpeed > MaxSpeed - 2)
            {
                AverageSpeed = MaxSpeed - 2;
            }
            if(AverageCarsPerMinute < 3)
            {
                AverageCarsPerMinute = 3;
            }
        }

        public int AverageSpeed { get; set; }
        public int MaxSpeed { get; set; }
        public int MinSpeed { get; set; }
        public int AverageCarsPerMinute { get; set; }
        public int SpeedingPercentage { get; set; }
        public IEnumerable<TimePeriod> RushHours { get; set; }
        public int SpeedLimit { get; set; }


        public bool IsRushHour(DateTime timeToEvaluate, out TimePeriod currentRushHour)
        {
            currentRushHour = RushHours.FirstOrDefault(rushHour => rushHour.Includes(timeToEvaluate));
            return currentRushHour != null;
        }
    }
}