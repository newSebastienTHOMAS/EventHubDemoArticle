using DemoEventHubIngestion;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DemoEventHubClient.Startup))]
namespace DemoEventHubClient
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IEventHubBroadCaster, EventHubBroadCaster>();
        }
    }
}
