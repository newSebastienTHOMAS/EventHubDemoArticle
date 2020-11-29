using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using DemoEventHubIngestion.Model;
using DemoEventHubClient.Model;

namespace DemoEventHubClient
{
    public class BroadCastMessage
    {
        private IEventHubBroadCaster _EvtHubBroadCaster;

        public BroadCastMessage(IEventHubBroadCaster eventHubBroadCaster)
        {
            _EvtHubBroadCaster = eventHubBroadCaster;
        }

        [FunctionName("BroadCastMessage")]
        public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] InputBody input, 
                ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request. SleepTime : {0}", input.SleepTime);
            

            var MessageToSend = new MessageEvent() {

                DateMessage = DateTime.Now,
                SleepTime = input.SleepTime
                
            };
                        
            // Broadcasting the body received to the Event Hub
            await _EvtHubBroadCaster.SerializeAndBroadCastMessageAsync(MessageToSend);


            return new OkObjectResult("Message Sent");
        }
    }
}
