using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TrafficCameraEventGenerator.Cars;
using TrafficCameraEventGenerator.Configuration;

namespace TrafficCameraEventGenerator.Transmitters
{
    public class ConsoleTransmitter : IEventTransmitter
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void Dispose()
        {
        }

        public ConsoleColor ConsoleColor { get; set; } = ConsoleColor.Yellow;

        public Task Transmit(CameraEvent cameraEvent, CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor;
            Console.WriteLine(cameraEvent.ToJson());
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
