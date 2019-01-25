using System;
using System.Collections.Generic;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Configuration.Segment;

namespace TrafficCameraEventGenerator.Cars
{
    public class SimulatedCar
    {
        public string LicensePlate { get; set; }
        public int ExpectedSpeed { get; set; }
        public string Color { get; set; }
        public string Make { get; set; }
        public bool Speeding { get; set; }
        public string Country { get; set; }

        public TimeSpan CalculateTime(int metersToDrive, int simulationMultiplier)
        {
            // Returns waiting time in milli seconds
            // applying the speed of the car
            // the distance of the cameras and the multiplier
            var metersPerHour = ExpectedSpeed * 1000;
            var metersPerMinute = metersPerHour / 60;
            double minutesToDrive = metersToDrive / (double)metersPerMinute;
            var millisecondsToDrive = ((minutesToDrive) * 60 * 1000) / simulationMultiplier;
            var result = TimeSpan.FromMilliseconds(Convert.ToInt32(millisecondsToDrive)); // in milliseconds
            return result;
        }


        private static List<string> Colors
        {
            get
            {
                var colors = new List<string>();
                foreach (var color in Enum.GetValues(typeof(ConsoleColor)))
                {
                    colors.Add(color.ToString());
                }

                return colors;
            }
        }
        private static readonly List<string> Makes = new List<string> { "Opel", "Audi", "Mercedes", "Volvo", "BMW", "Volkswagen", "Saab", "Renault", "Mazda", "Toyota", "Suzuki" };
        private static readonly List<string> Countries = new List<string> { "SK", "DK", "IT", "DE", "NL", "BE", "BE", "BE", "BE", "BE", "FR", "DE", "PL", "HU", "PT" };

        ///// <summary>
        ///// Gets random car
        ///// </summary>
        ///// <param name="random">Randomizer used to generate random values</param>
        ///// <param name="speeding">Is the car driving faster than the limit?</param>
        ///// <returns></returns>
        //public static SimulatedCar Randomize(Random random, bool speeding, int minSpeed, int speedLimit, int maxSpeed, bool isRushHour, string licensePlateFormat = "1-###-000")
        //{
        //    var car = new SimulatedCar
        //    {
        //        LicensePlate = random.GetString(licensePlateFormat),
        //        Color = Colors[random.Next(Colors.Count)],
        //        Make = Makes[random.Next(Makes.Count)],
        //        Country = Countries[random.NextTriangularInteger(0, Countries.Count - 1, Convert.ToInt32(Countries.Count / 2))], // weighted randomization (to get more belgian cars)
        //        Speeding = speeding
        //    };
        //    if (!isRushHour)
        //    {
        //        // No rushhour, so assume cars are driving faster and increase minimum speed
        //        minSpeed = Convert.ToInt32((minSpeed + speedLimit) * 0.75D); // No rushhour, so increase min speed
        //    }
        //    else
        //    {
        //        // Rushhour, so assume cars are driving slower and decrease maximum speed and disable speeding
        //        car.Speeding = car.Speeding && random.Next(1, 20) % 19 == 0;  //only 5% of these cases will be in speeding;
        //    }

        //    if (maxSpeed < speedLimit)
        //    {
        //        // Specific case to simulate traffic jam
        //        car.ExpectedSpeed = random.NextTriangularInteger(minSpeed, maxSpeed, speedLimit);
        //        car.Speeding = false;
        //    }
        //    else
        //    {
        //        car.ExpectedSpeed = speeding
        //            ? random.NextTriangularInteger(speedLimit, maxSpeed, speedLimit + 15) // Generate speeding cars with weight of speeding around 15 more than allowed
        //            : random.NextTriangularInteger(minSpeed, speedLimit, speedLimit - 5); // Generate non speeding cars, with weight closest to maxAllowed
        //    }
        //    return car;
        //}

        /// <summary>
        /// Gets random car
        /// </summary>
        /// <param name="random">Randomizer used to generate random values</param>
        /// <param name="speeding">Is the car driving faster than the limit?</param>
        /// <returns></returns>
        public static SimulatedCar Randomize(Random random, TrafficSegmentSituation segmentSituation, string licensePlateFormat = "1-###-000")
        {
            var car = new SimulatedCar
            {
                LicensePlate = random.GetString(licensePlateFormat),
                Color = Colors[random.Next(Colors.Count)],
                Make = Makes[random.Next(Makes.Count)],
                Country = Countries[random.NextTriangularInteger(0, Countries.Count - 1, Convert.ToInt32(Countries.Count / 2))], // weighted randomization (to get more belgian cars)
                Speeding = random.GetBooleanWithProbability(segmentSituation.SpeedingPercentage),
            };
            car.ExpectedSpeed = car.Speeding
                ? random.NextTriangularInteger(segmentSituation.SpeedLimit, segmentSituation.MaxSpeed, segmentSituation.SpeedLimit + 15) // Generate speeding cars with weight of speeding around 15 more than allowed
                : random.NextTriangularInteger(segmentSituation.MinSpeed, segmentSituation.SpeedLimit, segmentSituation.AverageSpeed); // Generate non speeding cars, with weight closest to maxAllowed
            return car;
        }
    }
}
