using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DemoEventHubIngestion.Model;
using DemoEventHubIngestion.Service;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DemoEventHubIngestion
{
    public class ProcessEventHubParallelized
    {
        private readonly TelemetryClient _clientTMetry;
        private readonly ILogger<ProcessEventHubParallelized> _logger;
        private IProcessMessage _processMessage;

        public ProcessEventHubParallelized(TelemetryConfiguration configuration,
                                                    ILogger<ProcessEventHubParallelized> log,
                                                    IProcessMessage processMessage)
        {
            _clientTMetry = new TelemetryClient(configuration);
            _logger = log;
            _processMessage = processMessage;
        }

        [FunctionName("IngestMessagesParallel")]
        public async Task Run([EventHubTrigger("eventhubinstancedemo", Connection = "eventHubConnectionString")] EventData[] events)
        {
            var exceptions = new List<Exception>();
            List<Task> AsyncTasks = new List<Task>();

            var timeStart = DateTime.Now;
            var id = Guid.NewGuid().ToString();
            var nbExceptions = 0;
            var nbSuccess = 0;
            var NbMessages = events.Length;
            _logger.LogInformation("Processing {nbMessages} messages", NbMessages);

            // Loop to launch all the calls Async
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                AsyncTasks.Add(_processMessage.ProcessEventHubRequest(messageBody));
            }

            _logger.LogInformation("Tasks launched for {nbMessages} messages", NbMessages);

            // Loop to wait for all the calls that have been launched
            while (AsyncTasks.Count > 0)
            {
                // Identify the first task that completes and create a task to the completion of found task.
                Task firstFinishedTask = await Task.WhenAny(AsyncTasks);

                // Remove the selected task from the list so that you don't process it more than once.
                AsyncTasks.Remove(firstFinishedTask);

                try
                {
                    // Await the completed task.
                    await firstFinishedTask;
                    if (firstFinishedTask.Exception != null)
                    {
                        foreach (var e in firstFinishedTask.Exception.InnerExceptions)
                        { exceptions.Add(e); }
                    }
                }
                catch (Exception ex)
                {
                    // Someting went wront when handling this task but we do not want to end the process of other tasks
                    _logger.LogError(ex, "Something went wrong");
                }
            }
            _logger.LogInformation("End Processing {nbMessages} for messages", NbMessages);

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.            
            var timeEnd = DateTime.Now;
            var elapsedtime = timeEnd.Subtract(timeStart);


            var evt = new EventTelemetry();
            evt.Name = "Process Events Batch Runner";
            evt.Properties["Batch Id"] = id;
            // /1000 => to get in seconds
            evt.Metrics["Messages Average Latency"] = (elapsedtime.TotalMilliseconds / NbMessages) / 1000;
            evt.Metrics["Messages Success"] = nbSuccess > 0 ? nbSuccess : 0;
            evt.Metrics["Messages Exceptions"] = nbExceptions;

            evt.Metrics["Messages Total"] = NbMessages;
            evt.Metrics["Batch Elapsed Time"] = ((TimeSpan)elapsedtime).TotalMilliseconds;

            _clientTMetry.TrackEvent(evt);

        }


    }
}
