using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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

            EventGridClient.SaveContext(context);

            try
            {
                log.LogInformation($"C# Event Hub trigger function processed a message: {suspectedCarMessage}");
                var suspectedCar = JsonConvert.DeserializeObject<SuspectedCarData>(suspectedCarMessage);
                var suspectedCarEvent =
                                    new SuspectedCarDetectedEvent(
                                        Guid.NewGuid().ToString("N"),
                                        $"traffic/{suspectedCar.TrajectId}",
                                        suspectedCar);
                await EventGridClient.Publisher.Publish(suspectedCarEvent);
            }
            catch (Exception e)
            {
                log.LogError(e, $"Error occurred when sending event grid messages: {e.Message}");
            }
        }
    }
}
