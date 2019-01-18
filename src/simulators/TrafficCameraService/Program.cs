using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
                cancellationTokenSource.Cancel();
            };



            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                //.AddSingleton<IConfigurationReader, EnvironmentConfigurationReader>()
                .AddSingleton<IConfigurationReader, HardcodedConfigurationReader>()
                .AddSingleton<ICameraTransmitterConfigurator, IoTHubTransmitterConfigurator>()
                //.AddSingleton<ITrafficSegmentConfigurator, TrafficSegmentSettingsConfigurator>()
                .AddSingleton<ITrafficSegmentConfigurator, BlobSegmentConfigurator>()
                .AddSingleton<ITimeSimulationSettings, TimeSimulationSettings>()
                .AddSingleton<IEventGenerator, EventGenerator>()
                .BuildServiceProvider();


            //do the actual work here
            var generator = serviceProvider.GetService<IEventGenerator>();
            generator.Run(cancellationTokenSource.Token).Wait(cancellationTokenSource.Token);
            exitEvent.WaitOne();
            Console.WriteLine("Process cancelled");
            Console.ReadLine();
        }
    }
}
