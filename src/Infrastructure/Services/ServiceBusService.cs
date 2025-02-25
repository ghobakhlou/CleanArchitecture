using Application.Common.Interfaces;
using MassTransit;
using Shared;
using Shared.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;
public class ServiceBusService(
        ICurrentUserService currentUserService,
        IPublishEndpoint publishEndpoint)
    : IServiceBusService
{
    public async Task SendAsync<T>(T message, CancellationToken cancellationToken) 
    {
        var baseMessage = new BaseMessage<T>(
             message,
             currentUserService.ObjectId,
             currentUserService.Email);

        await publishEndpoint.Publish(baseMessage, cancellationToken);
    }
}
