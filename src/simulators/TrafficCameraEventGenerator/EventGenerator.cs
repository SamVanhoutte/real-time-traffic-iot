using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;

namespace TrafficCameraEventGenerator
{
    public abstract class EventGenerator
    {
        protected readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly int _timeSimulationAccelerator;
        protected ITrafficSegmentIdentification SegmentIdentification { get; }
        protected ITrafficSegmentConfiguration Configuration { get; }
        protected abstract Task SubmitEvent(ICameraTransmitterConfiguration transmitConfiguration, CameraEvent cameraEvent);
        protected EventGenerator(ITrafficSegmentIdentification segmentIdentification, ITrafficSegmentConfiguration configuration, int timeSimulationAccelerator = 1)
        {
            _timeSimulationAccelerator = timeSimulationAccelerator;
            SegmentIdentification = segmentIdentification;
            Configuration = configuration;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var random = new Random();
            Parallel.For(1, Configuration.AverageCarsPerMinute + 1, async (index) =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Wait a random time between 0 seconds and 60 seconds (simulated!) 
                    // to start this task sending out cars
                    await Task.Delay(TimeSpan.FromMilliseconds((random.Next(60)) * 1000), cancellationToken);
                    while (!cancellationToken.IsCancellationRequested) // Keep running until task is cancelled
                    {
                        var numberOfCarsSpeeding = ((double)Configuration.AverageCarsPerMinute * ((double)Configuration.SpeedingPercentage / 100));
                        var isSpeeding = (index <= numberOfCarsSpeeding);
                        var car = SimulatedCar.Randomize
                            (
                                random: random,
                                speeding: isSpeeding,
                                minSpeed: Configuration.MinSpeed,
                                speedLimit: Configuration.SpeedLimit,
                                maxSpeed: Configuration.MaxSpeed
                            );
                        try
                        {
                            //regenerate new license plate for every run
                            var carTimespan = car.CalculateTime(Configuration.CameraDistance, _timeSimulationAccelerator);
                            await SubmitEvent(SegmentIdentification.InitialCamera,
                                new CameraEvent
                                {
                                    TrajectId = SegmentIdentification.SegmentId,
                                    CameraId = "cam-01",
                                    EventTime = SimulatedClock.GetTimestamp(),
                                    Car = car,
                                    Lane = LaneCalculator.CalculateLane(Configuration, car)
                                });
                            Logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 01");
                            await Task.Delay(carTimespan, cancellationToken);
                            await SubmitEvent(SegmentIdentification.InitialCamera,
                                new CameraEvent
                                {
                                    TrajectId = SegmentIdentification.SegmentId,
                                    CameraId = "cam-02",
                                    EventTime = SimulatedClock.GetTimestamp(),
                                    Car = car,
                                    Lane = LaneCalculator.CalculateLane(Configuration, car)
                                });
                            Logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 02");
                            // Wait to complete the simulatedMinute
                            var simulatedMinute = TimeSpan.FromMilliseconds(60000 / _timeSimulationAccelerator);
                            if (carTimespan < simulatedMinute)
                            {
                                await Task.Delay(simulatedMinute.Subtract(carTimespan), cancellationToken);
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Error happened in one of the simulation threads");
                        }
                    }
                }
                catch (TaskCanceledException)
                { }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error happened in the simulator");
                }
            });
        }
    }
}
