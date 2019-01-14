using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Savanh.Extensions.Randoms;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;
using TrafficCameraEventGenerator.Configuration.Segment;
using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Configuration.Simulation;
using TrafficCameraEventGenerator.Configuration.Transmission;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraEventGenerator
{
    public class EventGenerator : IEventGenerator
    {
        private readonly ITimeSimulationSettings _simulationSettings;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITrafficSegmentConfigurator _configurator;
        private readonly IConfigurationReader _configurationReader;
        private readonly ICameraTransmitterConfigurator _transmitterConfigurator;

        public EventGenerator(ITrafficSegmentConfigurator segmentConfiguration, IConfigurationReader configurationReader, ICameraTransmitterConfigurator transmitterConfigurator, ITimeSimulationSettings simulationSettings = null)
        {
            _configurator = segmentConfiguration;
            _configurationReader = configurationReader;
            _transmitterConfigurator = transmitterConfigurator;
            _simulationSettings = simulationSettings ?? new TimeSimulationSettings();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var segmentConfiguration = await _configurator.GetConfiguration();


            var startCameraEventTransmitter = _transmitterConfigurator.CreateTransmitter(CameraType.Camera1);
            var endCameraEventTransmitter = _transmitterConfigurator.CreateTransmitter(CameraType.Camera2);
            var random = new Random();
            // Initialize transmitters

            Parallel.For(1, segmentConfiguration.AverageCarsPerMinute + 1, async index =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Wait a random time between 0 seconds and 60 seconds (simulated!) 
                    // to start this task sending out cars
                    await Task.Delay(TimeSpan.FromMilliseconds((random.Next(60)) * 1000), cancellationToken);
                    while (!cancellationToken.IsCancellationRequested) // Keep running until task is cancelled
                    {
                        var numberOfCarsSpeeding = (segmentConfiguration.AverageCarsPerMinute * ((double)segmentConfiguration.SpeedingPercentage / 100));
                        var isSpeeding = (index <= numberOfCarsSpeeding);
                        var car = SimulatedCar.Randomize
                            (
                                random,
                                isSpeeding,
                                segmentConfiguration.MinSpeed,
                                segmentConfiguration.SpeedLimit,
                                segmentConfiguration.MaxSpeed
                            );
                        try
                        {
                            //regenerate new license plate for every run
                            var carTimespan = car.CalculateTime(segmentConfiguration.CameraDistance, _simulationSettings.TimeSimulationAccelerator);
                            await startCameraEventTransmitter.Transmit(
                                new CameraEvent
                                {
                                    TrajectId = segmentConfiguration.SegmentId,
                                    CameraId = CameraType.Camera1.ToString(),
                                    EventTime = SimulatedClock.GetTimestamp(),
                                    Car = car,
                                    Lane = LaneCalculator.CalculateLane(segmentConfiguration, car)
                                }, cancellationToken);
                            _logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 01");
                            await Task.Delay(carTimespan, cancellationToken);
                            await endCameraEventTransmitter.Transmit(
                                new CameraEvent
                                {
                                    TrajectId = segmentConfiguration.SegmentId,
                                    CameraId = CameraType.Camera2.ToString(),
                                    EventTime = SimulatedClock.GetTimestamp(),
                                    Car = car,
                                    Lane = LaneCalculator.CalculateLane(segmentConfiguration, car)
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
                    _logger.Error(ex, "Error happened in the simulator: " + ex.ToString());
                }
            });
        }
    }
}
