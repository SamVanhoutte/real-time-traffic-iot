using System;
using System.Threading.Tasks;
using EventGridTrigger.EventData;
using EventGridTrigger.Events;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventGridTrigger
{
    public static class SuspectedCarTrigger
    {
        [FunctionName("SuspectedCarTrigger")]
        public static async Task Run([EventHubTrigger("savanh-traffic-suspects", Connection = "suspected-cars-eventhub")]
            string suspectedCarMessage, ILogger log, ExecutionContext context)
        {

            ArcusEventGridClient.SaveContext(context);

            try
            {
                log.LogInformation($"C# Event Hub trigger function processed a message: {suspectedCarMessage}");
                var suspectedCar = JsonConvert.DeserializeObject<SuspectedCarData>(suspectedCarMessage);
                var suspectedCarEvent =
                                    new SuspectedCarDetectedEvent(
                                        Guid.NewGuid().ToString("N"),
                                        $"traffic/{suspectedCar.TrajectId}",
                                        suspectedCar);
                await ArcusEventGridClient.Publisher.Publish(suspectedCarEvent);
            }
            catch (Exception e)
            {
                log.LogError(e, $"Error occurred when sending event grid messages: {e.Message}");
            }
        }
    }
}
