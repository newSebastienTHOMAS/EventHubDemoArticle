# EventHubDemoArticle
Demo Projects for sending and Ingesting EventHub Messages with high performances


# This repo has two projects
* One Client for sending messages to an Event Hub 
* One Backend for ingesting messages from the Event Hub with a lots of metrics


# Usage
You only need to release the two projects on 2 Azure functions .net and modify these 3 configuration settings for each azure function :

* APPINSIGHTS_INSTRUMENTATIONKEY : this is to log events and custom metrics to an Application Insights Well of logs
* eventHubConnectionString : this is the connection strings with identication token to the event Hub
* eventHubEntityName : the name of the event Hub Entity Instance to connect (to send or ingest)
