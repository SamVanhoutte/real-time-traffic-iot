using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator
{
    public class EventGenerator<T> where T : IEventTransmitter, new()
    {
        private readonly ITimeSimulationSettings _simulationSettings;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITrafficSegmentIdentification _segmentIdentification;
        private readonly ITrafficSegmentConfiguration _configuration;
        private readonly T _startCameraEventTransmitter;
        private readonly T _endCameraEventTransmitter;

        public EventGenerator(ITrafficSegmentIdentification segmentIdentification, ITrafficSegmentConfiguration configuration, ITimeSimulationSettings simulationSettings = null)
        {
            _simulationSettings = simulationSettings ?? new TimeSimulationSettings();
            _segmentIdentification = segmentIdentification;
            _configuration = configuration;
            _startCameraEventTransmitter = Activator.CreateInstance<T>();
            _endCameraEventTransmitter = Activator.CreateInstance<T>();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var random = new Random();
            // Initialize transmitters
            await Task.WhenAll(
                _startCameraEventTransmitter.Initalize(_segmentIdentification.InitialCamera),
                _endCameraEventTransmitter.Initalize(_segmentIdentification.InitialCamera)
            );
            Parallel.For(1, _configuration.AverageCarsPerMinute + 1, async (index) =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Wait a random time between 0 seconds and 60 seconds (simulated!) 
                    // to start this task sending out cars
                    await Task.Delay(TimeSpan.FromMilliseconds((random.Next(60)) * 1000), cancellationToken);
                    while (!cancellationToken.IsCancellationRequested) // Keep running until task is cancelled
                    {
                        var numberOfCarsSpeeding = ((double)_configuration.AverageCarsPerMinute * ((double)_configuration.SpeedingPercentage / 100));
                        var isSpeeding = (index <= numberOfCarsSpeeding);
                        var car = SimulatedCar.Randomize
                            (
                                random,
                                isSpeeding,
                                _configuration.MinSpeed,
                                _configuration.SpeedLimit,
                                _configuration.MaxSpeed
                            );
                        try
                        {
                            //regenerate new license plate for every run
                            var carTimespan = car.CalculateTime(_configuration.CameraDistance, _simulationSettings.TimeSimulationAccelerator);
                            await _startCameraEventTransmitter.Transmit(
                                new CameraEvent
                                {
                                    TrajectId = _segmentIdentification.SegmentId,
                                    CameraId = "cam-01",
                                    EventTime = SimulatedClock.GetTimestamp(),
                                    Car = car,
                                    Lane = LaneCalculator.CalculateLane(_configuration, car)
                                }, cancellationToken);
                            _logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 01");
                            await Task.Delay(carTimespan, cancellationToken);
                            await _startCameraEventTransmitter.Transmit(
                                new CameraEvent
                                {
                                    TrajectId = _segmentIdentification.SegmentId,
                                    CameraId = "cam-02",
                                    EventTime = SimulatedClock.GetTimestamp(),
                                    Car = car,
                                    Lane = LaneCalculator.CalculateLane(_configuration, car)
                                }, cancellationToken);
                            _logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 02");
                            // Wait to complete the simulatedMinute
                            var simulatedMinute = TimeSpan.FromMilliseconds(60000 / _simulationSettings.TimeSimulationAccelerator);
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
                            _logger.Error(ex, "Error happened in one of the simulation threads");
                        }
                    }
                }
                catch (TaskCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error happened in the simulator");
                }
            });
        }
    }
}
