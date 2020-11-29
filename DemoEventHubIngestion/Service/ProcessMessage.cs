using DemoEventHubIngestion.Model;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoEventHubIngestion.Service
{
    public class ProcessMessageClass : IProcessMessage
    {
        private TelemetryClient _clientTMetry;
        private ILogger<ProcessMessageClass> _logger;

        public ProcessMessageClass(TelemetryConfiguration configuration,
                              ILogger<ProcessMessageClass> log)
        {
            _clientTMetry = new TelemetryClient(configuration);
            _logger = log;
        }

        public async Task ProcessEventHubRequest(string messageBody)
        {
            DateTime startTime = DateTime.Now;
            DateTime lattencyStartTime = DateTime.MinValue;
            string messageResult = "Success";
            try
            {
                var messageEvent = JsonConvert.DeserializeObject<MessageEvent>(messageBody);
                lattencyStartTime = messageEvent.DateMessage;
                _logger.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");

                // We wait one second to simulate an important workload 
                await Task.Delay(messageEvent.SleepTime);
            }
            catch (Exception ex)
            {
                messageResult = "Error";
            }
            finally
            {
                DateTime endTime = DateTime.Now;

                var evt = new EventTelemetry();
                evt.Name = "Process Single Event Hub Message ";
                evt.Properties["Message"] = messageResult;

                if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    var elapsedTime = endTime.Subtract(startTime);
                    evt.Metrics["Elapsed Process Time"] = elapsedTime.TotalMilliseconds;

                    if (lattencyStartTime != DateTime.MinValue)
                    {
                        var elapsedLattencyTime = startTime.Subtract(lattencyStartTime);
                        evt.Metrics["Lattency before starting process"] = elapsedTime.TotalMilliseconds;

                        var totalLattencyTime = endTime.Subtract(lattencyStartTime);
                        evt.Metrics["Total lattency"] = totalLattencyTime.TotalMilliseconds;
                    }
                }
                _clientTMetry.TrackEvent(evt);
            }
        }
    }
}
