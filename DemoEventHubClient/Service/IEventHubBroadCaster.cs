using System.Threading.Tasks;

namespace DemoEventHubClient
{
    public interface IEventHubBroadCaster
    {
        Task SerializeAndBroadCastMessageAsync(object objectToSend);
    }
}