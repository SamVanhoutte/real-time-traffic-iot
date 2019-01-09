using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCameraEventGenerator;
using TrafficCameraEventGenerator.Configuration;
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

            var generator = new EventGenerator<ConsoleTransmitter>(
                new TrafficSegmentIdentification
                {
                    InitialCamera = new CameraTransmitterConfiguration(""),
                    LastCamera = new CameraTransmitterConfiguration(""),
                    SegmentId = "demo-01"
                },
                TrafficSegmentConfiguration.Busy);
            generator.Run(cancellationTokenSource.Token).Wait(cancellationTokenSource.Token);
            exitEvent.WaitOne();
            Console.WriteLine("Process cancelled");
        }
    }
}
