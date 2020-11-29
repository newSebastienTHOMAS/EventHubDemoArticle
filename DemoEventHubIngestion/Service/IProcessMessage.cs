using System.Threading.Tasks;

namespace DemoEventHubIngestion.Service
{
    public interface IProcessMessage
    {
        Task ProcessEventHubRequest(string messageBody);
    }
}