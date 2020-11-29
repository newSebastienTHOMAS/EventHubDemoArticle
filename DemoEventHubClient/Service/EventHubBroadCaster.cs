using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DemoEventHubClient
{
    public class EventHubBroadCaster : IEventHubBroadCaster
    {
        
        private static readonly Lazy<EventHubClient> lazyevtHubClient = new Lazy<EventHubClient>(GetEventHubClient);
        private static EventHubClient EvtHubClient => lazyevtHubClient.Value;

        private static readonly string eventHubConnectionString = Environment.GetEnvironmentVariable(nameof(eventHubConnectionString));
        private static readonly string eventHubEntityName = Environment.GetEnvironmentVariable(nameof(eventHubEntityName));
        private ILogger<EventHubBroadCaster> _logger;

        public EventHubBroadCaster(ILogger<EventHubBroadCaster> log)
        {
            _logger = log;
        }
        



        private static EventHubClient GetEventHubClient()
        {            
            return EventHubClient.CreateFromConnectionString(
                                            new EventHubsConnectionStringBuilder(eventHubConnectionString)
                                            {
                                                EntityPath = eventHubEntityName
                                            }.ToString()
                                        );
        }
        
        public async Task SerializeAndBroadCastMessageAsync(object objectToSend)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objectToSend));

            if (bytes == null)
            {
                throw new ArgumentException("Invalid bytes lenth");
            }

            if (bytes.Length == 0)
            {
                throw new ArgumentException("Invalid bytes lenth");
            }
            
            await EvtHubClient.SendAsync(new EventData(bytes));

            
            _logger.LogInformation("BroadCast Single message");
        }
    }
}
