using DemoEventHubIngestion;
using DemoEventHubIngestion.Service;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DemoEventHubIngestion.Startup))]
namespace DemoEventHubIngestion
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IProcessMessage, ProcessMessageClass>();
        }
    }
}
