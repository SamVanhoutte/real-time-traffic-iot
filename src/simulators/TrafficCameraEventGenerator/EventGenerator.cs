using System;
using System.Linq;
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
            var segmentSituation = new TrafficSegmentSituation(segmentConfiguration);
            var startCameraEventTransmitter = await _transmitterConfigurator.CreateTransmitter(segmentConfiguration.SegmentId, CameraType.Camera1);
            var endCameraEventTransmitter = await _transmitterConfigurator.CreateTransmitter(segmentConfiguration.SegmentId, CameraType.Camera2);
            var random = new Random();
            // Initialize transmitters
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    segmentSituation.Resimulate(random);
                    var releaseInterval = TimeSpan.FromMilliseconds(Convert.ToInt32(60000 / segmentSituation.AverageCarsPerMinute)); // replace with function
                    var currentMinute = SimulatedClock.GetTimestamp().Minute;
                    while (SimulatedClock.GetTimestamp().Minute == currentMinute)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Task.Factory.StartNew(() => MakeOneCarDrive(random, segmentConfiguration, segmentSituation, startCameraEventTransmitter, endCameraEventTransmitter, cancellationToken), cancellationToken);
                        await Task.Delay(releaseInterval, cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException)
            { }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error happened in the simulator: " + ex.ToString());
            }
        }

        private async Task MakeOneCarDrive(Random random, TrafficSegmentConfiguration trafficConfiguration, TrafficSegmentSituation segmentSituation, IEventTransmitter startCameraEventTransmitter, IEventTransmitter endCameraEventTransmitter, CancellationToken cancellationToken)
        {
            // Wait random short interval to add randomness
            await Task.Delay(TimeSpan.FromMilliseconds(random.Next(2000)), cancellationToken);
            var car = SimulatedCar.Randomize
            (
                random, segmentSituation
            );
            try
            {
                //regenerate new license plate for every run
                var carTimespan = car.CalculateTime(trafficConfiguration.CameraDistance, _simulationSettings.TimeSimulationAccelerator);
                await startCameraEventTransmitter.Transmit(
                    new CameraEvent
                    {
                        TrajectId = trafficConfiguration.SegmentId,
                        CameraId = CameraType.Camera1.ToString(),
                        EventTime = SimulatedClock.GetTimestamp(),
                        Car = car,
                        Lane = LaneCalculator.CalculateLane(trafficConfiguration, segmentSituation, car)
                    }, cancellationToken);
                _logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 01");
                await Task.Delay(carTimespan, cancellationToken);
                await endCameraEventTransmitter.Transmit(
                    new CameraEvent
                    {
                        TrajectId = trafficConfiguration.SegmentId,
                        CameraId = CameraType.Camera2.ToString(),
                        EventTime = SimulatedClock.GetTimestamp(),
                        Car = car,
                        Lane = LaneCalculator.CalculateLane(trafficConfiguration, segmentSituation, car)
                    }, cancellationToken);
                _logger.Trace($"{car.Color} {car.Make} with license plate {car.LicensePlate} detected by camera 02");
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error happened in one of the simulation threads");
            }
        }
    }
}
