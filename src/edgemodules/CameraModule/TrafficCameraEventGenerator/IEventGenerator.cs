using System.Threading;
using System.Threading.Tasks;

namespace TrafficCameraEventGenerator
{
    public interface IEventGenerator
    {
        Task Run(CancellationToken cancellationToken);
    }
}