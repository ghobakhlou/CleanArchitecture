using Shared.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;
public interface IServiceBusService
{
    Task SendAsync<T>(T message, CancellationToken cancellationToken = default) ;
}