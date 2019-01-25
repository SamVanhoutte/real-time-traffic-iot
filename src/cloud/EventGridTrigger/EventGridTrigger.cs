
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arcus.EventGrid.Publishing;
using Arcus.EventGrid.Publishing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventGridTrigger
{
    public static class EventGridTrigger
    {
        [FunctionName("EventGridTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string triggerType = req.Query["trigger-type"];
            _functionContext = context;

            var events = new List<CarSpeedingEvent> { };

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            log.LogInformation(requestBody);
            var speedingList = JsonConvert.DeserializeObject<IEnumerable<CarSpeedingData>>(requestBody);
            events.AddRange(speedingList.Select(carSpeedingData => 
                new CarSpeedingEvent(
                    Guid.NewGuid().ToString("N"), 
                    $"traffic/{carSpeedingData.TrajectId}", 
                    carSpeedingData)));
            await Publisher.PublishMany(events);

            return triggerType != null
                ? (ActionResult)new OkObjectResult("received")
                : new BadRequestObjectResult("Trigger type was not specified in the query string (trigger-type)");
        }

        private static IEventGridPublisher _publisher;
        private static ExecutionContext _functionContext;

        private static IEventGridPublisher Publisher
        {
            get
            {
                if (_publisher != null) return _publisher;

                var config = new ConfigurationBuilder()
                    .SetBasePath(_functionContext.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                _publisher = EventGridPublisherBuilder
                    .ForTopic(config["event-grid-topic-name"])
                    .UsingAuthenticationKey(config["event-grid-authentication-key"])
                    .WithExponentialRetry<Exception>(3)
                    .Build();
                return _publisher;
            }
        }
    }
}
