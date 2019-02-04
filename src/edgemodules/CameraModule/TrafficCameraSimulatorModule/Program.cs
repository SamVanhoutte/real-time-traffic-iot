using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrafficCameraEventGenerator;
using TrafficCameraEventGenerator.Configuration;
using TrafficCameraEventGenerator.Configuration.Segment;
using TrafficCameraEventGenerator.Configuration.Settings;
using TrafficCameraEventGenerator.Configuration.Simulation;
using TrafficCameraEventGenerator.Configuration.Transmission;
using TrafficCameraEventGenerator.Transmitters;

namespace TrafficCameraService
{
    class Program
    {
        private static IEventGenerator _generator;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();


        static void Main(string[] args)
        {
            Init().Wait();

            var exitEvent = new ManualResetEvent(false);
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
                cancellationTokenSource.Cancel();
            };

            AssemblyLoadContext.Default.Unloading += (ctx) => cancellationTokenSource.Cancel();

            //do the actual work here
            _generator.Run(cancellationTokenSource.Token).Wait(cancellationTokenSource.Token);
            exitEvent.WaitOne();
            WhenCancelled(cancellationTokenSource.Token).Wait();
            Console.WriteLine("Process cancelled");
            Console.ReadLine();
        }

        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        private static async Task Init()
        {
            //setup our DI
            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfigurationReader, EnvironmentConfigurationReader>()
                .AddSingleton<ITrafficSegmentConfigurator, BlobSegmentConfigurator>()
                .AddSingleton<ITimeSimulationSettings, TimeSimulationSettings>()
                .AddSingleton<IEventGenerator, EventGenerator>();
            if (IsEdgeEnvironment)
            {
                _logger.Info($"This container is running in edge environment, creating iot edge module transmitter");
                serviceCollection.AddSingleton<ICameraTransmitterConfigurator, IoTEdgeModuleTransmitterConfiguration>();
            }
            else
            {
                _logger.Info($"This container is running in edge environment, creating iot hub transmitter");
                serviceCollection.AddSingleton<ICameraTransmitterConfigurator, IoTHubTransmitterConfigurator>();
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _generator = serviceProvider.GetService<IEventGenerator>();
        }

        private static bool IsEdgeEnvironment => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("IOTEDGE_MODULEID"));
    }
}
