using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Azure.Messaging.EventHubs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;

namespace DemoEventHubClient
{
    public class EventHubBroadCaster : IEventHubBroadCaster
    {
        
        private static readonly Lazy<EventHubProducerClient> lazyevtHubClient = new Lazy<EventHubProducerClient>(GetEventHubClient);
        private static EventHubProducerClient EvtHubClient => lazyevtHubClient.Value;

        private static readonly string eventHubConnectionString = Environment.GetEnvironmentVariable(nameof(eventHubConnectionString));
        private static readonly string eventHubEntityName = Environment.GetEnvironmentVariable(nameof(eventHubEntityName));
        private ILogger<EventHubBroadCaster> _logger;

        public EventHubBroadCaster(ILogger<EventHubBroadCaster> log)
        {
            _logger = log;
        }
        



        private static EventHubProducerClient GetEventHubClient()
        {
            return new EventHubProducerClient(eventHubConnectionString, eventHubEntityName);                                        
        }
        
        public async Task SerializeAndBroadCastMessageAsync(object objectToSend)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objectToSend));

            using EventDataBatch eventBatch = await EvtHubClient.CreateBatchAsync();
            if (bytes == null)
            {
                throw new ArgumentException("Invalid bytes lenth");
            }

            if (bytes.Length == 0)
            {
                throw new ArgumentException("Invalid bytes lenth");
            }
            eventBatch.TryAdd(new EventData(bytes));
            await EvtHubClient.SendAsync(eventBatch);

            
            _logger.LogInformation("BroadCast Single message");
        }
    }
}
