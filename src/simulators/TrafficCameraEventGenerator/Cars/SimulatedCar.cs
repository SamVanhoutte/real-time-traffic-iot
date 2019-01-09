using System;
using System.Collections.Generic;
using Savanh.Extensions.Randoms;

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
            var result = TimeSpan.FromMilliseconds((int)(millisecondsToDrive)); // in milliseconds
            return result;
        }


        private static List<string> Colors {
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
        private static readonly List<string> Makes = new List<string> { "Opel", "Audi", "Mercedes", "Volvo", "BMW", "Volkswagen", "Saab", "Renault", "Mazda", "Toyota", "Suzuki"};
        private static readonly List<string> Countries = new List<string> { "BE", "BE", "BE", "BE", "BE", "BE", "BE", "BE", "BE", "NL", "FR" , "PT", "FR", "NL", "IT", "IT"};

        /// <summary>
        /// Gets random car
        /// </summary>
        /// <param name="random">Randomizer used to generate random values</param>
        /// <param name="speeding">Is the car driving faster than the limit?</param>
        /// <returns></returns>
        public static SimulatedCar Randomize(Random random, bool speeding, int minSpeed, int speedLimit, int maxSpeed, string licensePlateFormat = "1-###-000")
        {
            var car = new SimulatedCar
            {
                LicensePlate = random.GetString(licensePlateFormat),
                Color = Colors[random.Next(Colors.Count)],
                Make = Makes[random.Next(Makes.Count)],
                Country = Countries[random.Next(Countries.Count)],
                Speeding = speeding
            };
            // Oh boy, I'm sure this can be better, but it's way too late now 
            if (maxSpeed < speedLimit)
            {
                // Specific case to simulate traffic jam
                car.ExpectedSpeed = random.Next(minSpeed, maxSpeed);
                car.Speeding = false;
            }
            else
            {
                car.ExpectedSpeed = speeding ? random.Next(speedLimit, maxSpeed) : random.Next(minSpeed, speedLimit);
            }
            return car;
        }
    }
}
