
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
            EventGridClient.SaveContext(context);

            var events = new List<CarSpeedingEvent> { };
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                log.LogInformation(requestBody);
                var speedingList = JsonConvert.DeserializeObject<IEnumerable<CarSpeedingData>>(requestBody);
                events.AddRange(speedingList.Select(carSpeedingData =>
                    new CarSpeedingEvent(
                        Guid.NewGuid().ToString("N"),
                        $"traffic/{carSpeedingData.TrajectId}",
                        carSpeedingData)));
                await EventGridClient.Publisher.PublishMany(events);

                return (ActionResult)new OkObjectResult("received");
                    

            }
            catch (Exception e)
            {
                log.LogError(e, $"Error occurred when sending event grid messages: {e.Message}" );
                return new InternalServerErrorResult();
            }
        }


    }
}
